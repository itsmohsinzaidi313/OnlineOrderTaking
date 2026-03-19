using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;
using StackExchange.Redis;
using System.Text.Json;

namespace GatewayService.ServiceResponseListeners
{
    public class CreateOrderServiceResponseListener(ILogger<CreateOrderServiceResponseListener> logger, RabbitMqConnection rabbitConnection, Implementation implementation, IConnectionMultiplexer redis) : RabbitMqConsumerService<CreateOrderServiceResponseListener>(logger, rabbitConnection)
    {
        public override string QueueName() => RabbitMqQueues.OrderResponseQueue;
        public override async Task OnMessage(string svcPayload)
        {
            await implementation.SendToUser<OrderServicePayload>(svcPayload);
            using var doc = JsonDocument.Parse(svcPayload);
            var root = doc.RootElement;
            var clientId = root.GetProperty("UserId").GetString() ?? throw new Exception("UserId not found");
            var orderNumber = root.GetProperty("DataPayload").GetProperty("OrderNumber").GetString() ?? throw new Exception("OrderNumber not found");
            var db = redis.GetDatabase();
            await db.StringSetAsync($"order:{orderNumber}:{clientId}", svcPayload, expiry: TimeSpan.FromHours(2));
        }
    }
}
