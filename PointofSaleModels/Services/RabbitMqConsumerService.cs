using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Logging;

namespace PointofSaleModels.Services
{
    public class RabbitMqConsumerService(ILogger<RabbitMqConsumerService> logger, RabbitMqConnection rabbitConnection, IQueueExecution exec) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await rabbitConnection.InitializeAsync();
            var channel = rabbitConnection.Channel;
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
        }
    }
}
