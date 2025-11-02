using System.Text;
using System.Text.Json;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;

namespace PointofSaleModels.Services
{
    public abstract class RabbitMqConsumerService<T>(ILogger<T> logger, RabbitMqConnection rabbitConnection, IQueueAction exec) : BackgroundService
    {
        private readonly string QueueName = exec.QueueName();
        public async Task Build(CancellationToken stoppingToken)
        {
            await rabbitConnection.InitializeAsync();
            var channel = rabbitConnection.Channel!;
            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (sender, ea) =>
           {
               var message = Encoding.UTF8.GetString(ea.Body.ToArray());
               logger.LogInformation("📥 Received request: {Message}", message);
               var obj = JsonSerializer.Deserialize<RabbitMqTransport>(message);
               if (obj == null)
               {
                   logger.LogWarning("⚠️ Received null or invalid message.");
                   return;
               }
               await exec.OnMessage(obj);
               await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
           };
            var settings = rabbitConnection._settings;

            await rabbitConnection.EnsureQueueExistsAsync(QueueName);

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
        }
    }
}

