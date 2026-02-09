using GatewayService.Interfaces;
using GatewayService;
using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;
using StackExchange.Redis;
using System.Text.Json;

namespace GatewayService.ServiceResponseListeners
{
    public class DataServiceResponseAction(Implementation implementation, IConnectionMultiplexer redis, ApiDataRequestCoordinator apiResponses) : IDataServiceResponseAction
    {
        public string QueueName() => RabbitMqQueues.DataResponseQueue;
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

            if (!string.IsNullOrWhiteSpace(correlationId) && apiResponses.TryResolve(correlationId, svcPayload))
            {
                return;
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
