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
            await Task.Delay(1000);
            using var doc = JsonDocument.Parse(svcPayload);
            var root = doc.RootElement;
            var clientId = root.GetProperty("UserId").GetString() ?? throw new Exception("UserId not found");
            var orderNumber = root.GetProperty("DataPayload").GetProperty("OrderNumber").GetString() ?? throw new Exception("OrderNumber not found");
            var signalrMethodName = root.GetProperty("SignalRMethodName").GetString() ?? throw new Exception("SignalRMethodName not found");
            logger.LogInformation("Received order response for order {OrderNumber} and user {UserId}\n{svcPayload}", orderNumber, clientId, svcPayload);
            for (int i = 0; i < 3; i++)
            {
                await implementation.SendToUser<OrderServicePayload>(svcPayload);
            }
            var db = redis.GetDatabase();
            await db.StringSetAsync($"order:{orderNumber}:{clientId}", svcPayload, expiry: TimeSpan.FromHours(2));
        }
    }
}
