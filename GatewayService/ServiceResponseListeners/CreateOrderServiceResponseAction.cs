using GatewayService.Interfaces;
using PointofSaleModels.Application;
using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Settings;
using StackExchange.Redis;
using System.Text.Json;

namespace GatewayService.ServiceResponseListeners
{
    public class CreateOrderServiceResponseAction(Implementation implementation, IConnectionMultiplexer redis) : ICreateOrderResponseAction
    {
        public string QueueName() => RabbitMqQueues.OrderResponseQueue;
        public async Task OnMessage(string svcPayload)
        {
            using var doc = JsonDocument.Parse(svcPayload);
            var root = doc.RootElement;
            var userId = root.GetProperty("UserId").GetInt32();
            var dataPayload = root.GetProperty("DataPayload");
            var success = dataPayload.GetProperty("Success").GetBoolean();
            if (success)
            {
                var db = redis.GetDatabase();
                var server = redis.GetServer(redis.GetEndPoints().First());
                var keys = server.Keys(pattern: $"branch:{userId}:*:connection");
                var connctionIds = new List<string>();
                foreach (var key in keys)
                {
                    var connectionId = await db.StringGetAsync(key);
                    if (!connectionId.IsNullOrEmpty)
                    {
                        connctionIds.Add(connectionId.ToString());
                    }
                }
                var customerOrderProperty = dataPayload.GetProperty("Order");
                var customerOrder = JsonSerializer.Deserialize<CustomerOrder>(customerOrderProperty.GetRawText())!;
                await implementation.SendCustomerOrderToBranches(customerOrder, connctionIds);
            }
            await implementation.SendToUser<OrderServicePayload>(svcPayload);
        }
    }
}
