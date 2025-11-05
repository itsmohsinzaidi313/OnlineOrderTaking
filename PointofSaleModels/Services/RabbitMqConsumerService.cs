using System.Text;
using System.Text.Json;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;

namespace PointofSaleModels.Services
{
    public abstract class RabbitMqConsumerService<T>(ILogger<T> logger, RabbitMqConnection rabbitConnection, IQueueAction exec) : BackgroundService
    {
        private readonly string QueueName = exec.QueueName();
        public async Task Build(CancellationToken stoppingToken)
        {
            await rabbitConnection.InitializeAsync();
            var channel = await rabbitConnection.CreateChannelAsync();
            // Apply a reasonable prefetch to improve throughput and fairness
            await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 10, global: false, cancellationToken: stoppingToken);
            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (sender, ea) =>
           {
               var message = Encoding.UTF8.GetString(ea.Body.ToArray());
               logger.LogInformation("📥 Received request: {Message}", message);
               RabbitMqTransport? obj = null;
               try
               {
                   obj = JsonSerializer.Deserialize<RabbitMqTransport>(message, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                   if (obj == null)
                   {
                       logger.LogWarning("⚠️ Received null or invalid message.");
                       await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
                       return;
                   }
                   await exec.OnMessage(obj);
                   await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
               }
               catch (OperationCanceledException)
               {
                   await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
               }
               catch (Exception ex)
               {
                   logger.LogError(ex, "❌ Error processing message from {Queue}", QueueName);
                   await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
               }
           };
            await RabbitMqConnection.EnsureQueueExistsAsync(channel, QueueName);

            await channel.BasicConsumeAsync(
                queue: QueueName,
                autoAck: false,
                consumerTag: string.Empty,
                noLocal: false,
                exclusive: false,
                arguments: null,
                consumer: consumer,
                cancellationToken: stoppingToken
            );

            try
            {
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // expected on shutdown
            }
            finally
            {
                try { await channel.CloseAsync(); } catch { /* ignore */ }
            }
        }
    }
}

