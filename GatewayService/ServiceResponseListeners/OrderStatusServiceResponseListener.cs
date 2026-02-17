using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;
using StackExchange.Redis;
using System.Text.Json;

namespace GatewayService.ServiceResponseListeners
{
    public class OrderStatusServiceResponseListener(ILogger<OrderStatusServiceResponseListener> logger, RabbitMqConnection rabbitConnection, Implementation implementation, IConnectionMultiplexer redis) : RabbitMqConsumerService<OrderStatusServiceResponseListener>(logger, rabbitConnection)
    {
        public override string QueueName() => RabbitMqQueues.OrderStatusResponseQueue;
        public override async Task OnMessage(string svcPayload)
        {
            await implementation.SendToUser<OrderStatusPayload>(svcPayload);
            using var doc = JsonDocument.Parse(svcPayload);
            var root = doc.RootElement;
            var orderNumber = root.GetProperty("OrderNumber").GetString() ?? throw new Exception("OrderNumber not found");
            var db = redis.GetDatabase();
            var server = redis.GetServer(redis.GetEndPoints().First());
            var payload = JsonSerializer.Deserialize<OrderStatusPayload>(svcPayload);
            foreach (var key in server.Keys(pattern: $"order:{orderNumber}:*"))
            {
                var clientId = key.ToString().Split(':')[2];
                if (string.IsNullOrEmpty(clientId))
                {
                    continue;
                }
                await implementation.SendToUser<OrderStatusPayload>("OrderStatusUpdate", clientId, payload);
            }
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Build(stoppingToken);
        }
    }
}
