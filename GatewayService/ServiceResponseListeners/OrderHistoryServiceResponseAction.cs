using GatewayService.Interfaces;
using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Settings;
using StackExchange.Redis;
using System.Text.Json;

namespace GatewayService.ServiceResponseListeners
{
    public class OrderHistoryServiceResponseAction(Implementation implementation, IConnectionMultiplexer redis) : IOrderHistoryResponseAction
    {
        public string QueueName() => RabbitMqQueues.OrderHistoryResponseQueue;
        public async Task OnMessage(string svcPayload)
        {
            using var doc = JsonDocument.Parse(svcPayload);
            var root = doc.RootElement;
            var correlationId = root.TryGetProperty("CorrelationId", out var correlationProp) ? correlationProp.GetString() : null;
            var dataRequestType = root.GetProperty("DataRequestType").GetString() ?? throw new Exception("Unknown request type");
            var branchId = root.GetProperty("BranchId").GetInt32();
            var domainName = root.GetProperty("DomainName").GetString() ?? throw new Exception("DomainName not found");
            var dataPayloadElement = root.GetProperty("DataPayload");
            var success = root.GetProperty("Success").GetBoolean();
            var rediKey = dataRequestType switch
            {
                "Menu" => "Menu",
                "DeliveryAndPickup" => "DAndP",
                _ => string.Empty
            };
            if (success && !string.IsNullOrEmpty(rediKey))
            {
                await redis.GetDatabase().StringSetAsync($"{domainName}:{branchId}:{rediKey}", svcPayload);
            }

            if (!success)
            {
                await implementation.SendToUser<DataServicePayload>(svcPayload);
                return;
            }

            await implementation.SendToUser<DataServicePayload>(svcPayload);
        }
    }
}
