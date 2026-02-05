using GatewayService.Interfaces;
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
            var branchId = root.GetProperty("BranchId").GetInt32();
            var success = root.GetProperty("DataPayload").GetProperty("Success").GetBoolean();
            if (success)
            {
                var db = redis.GetDatabase();
                var server = redis.GetServer(redis.GetEndPoints().First());
                var keys = server.Keys(pattern: $"branch:{branchId}:*");
                await implementation.SendToBranches<OrderServicePayload>(svcPayload, [.. keys.Select(x => x.ToString())]);
            }
            await implementation.SendToUser<OrderServicePayload>(svcPayload);
        }
    }
}
