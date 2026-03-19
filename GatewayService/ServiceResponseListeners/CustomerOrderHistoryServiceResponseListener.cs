using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;
using StackExchange.Redis;

namespace GatewayService.ServiceResponseListeners
{
    public class CustomerOrderHistoryServiceResponseListener(ILogger<CustomerOrderHistoryServiceResponseListener> logger, RabbitMqConnection rabbitConnection, Implementation implementation, IConnectionMultiplexer redis) : RabbitMqConsumerService<CustomerOrderHistoryServiceResponseListener>(logger, rabbitConnection)
    {
        public override string QueueName() => RabbitMqQueues.CustomerOrderHistoryResponseQueue;
        public override async Task OnMessage(string svcPayload)
        {
            await implementation.SendToUser<CustomerOrderHistoryServicePayload>(svcPayload);
        }
    }
}
