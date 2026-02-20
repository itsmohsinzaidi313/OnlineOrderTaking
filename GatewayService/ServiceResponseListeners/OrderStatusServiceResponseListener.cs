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
            var payload = JsonSerializer.Deserialize<OrderStatusPayload>(svcPayload);
            var server = redis.GetServer(redis.GetEndPoints().First());

            foreach (var key in server.Keys(pattern: $"branch:*:*:connection"))
            {
                var arr = key.ToString().Split(':');
                var clientId = $"{arr[0]}:{arr[2]}:{arr[2]}";
                if (string.IsNullOrEmpty(clientId))
                {
                    continue;
                }
                var responseKey = payload?.ResponseKey ?? throw new Exception("ResponseKey not found");
                await implementation.SendToUser<OrderStatusPayload>(responseKey, clientId, payload);
            }

            var orderNumber = payload?.OrderNumber ?? throw new Exception("OrderNumber not found");
            foreach (var key in server.Keys(pattern: $"order:{orderNumber}:*"))
            {
                var arr = key.ToString().Split(':');
                var clientId = $"{arr[2]}:{arr[3]}:{arr[4]}";
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
