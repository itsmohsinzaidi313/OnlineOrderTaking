using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;
using StackExchange.Redis;
using System.Text.Json;

namespace GatewayService.ServiceResponseListeners
{
    public class SettingsDataServiceResponseListener(ILogger<SettingsDataServiceResponseListener> logger, RabbitMqConnection rabbitConnection, Implementation implementation, IConnectionMultiplexer redis) : RabbitMqConsumerService<SettingsDataServiceResponseListener>(logger, rabbitConnection)
    {
        public override string QueueName() => RabbitMqQueues.SettingResponseQueue;
        public override async Task OnMessage(string svcPayload)
        {
            using var doc = JsonDocument.Parse(svcPayload);
            var root = doc.RootElement;
            var branchId = root.GetProperty("BranchId").GetInt32();
            var domainName = root.GetProperty("DomainName").GetString() ?? throw new Exception("DomainName not found");
            var success = root.GetProperty("Success").GetBoolean();
            if (success)
            {
                await redis.GetDatabase().StringSetAsync($"{domainName}:{branchId}:dandp", svcPayload);
            }

            await implementation.SendToUser<DataServicePayload>(svcPayload);
        }
    }
}
