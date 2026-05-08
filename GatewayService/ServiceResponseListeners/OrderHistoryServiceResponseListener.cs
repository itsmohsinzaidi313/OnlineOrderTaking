using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;

namespace GatewayService.ServiceResponseListeners
{
    public class OrderHistoryServiceResponseListener(ILogger<OrderHistoryServiceResponseListener> logger, RabbitMqConnection rabbitConnection, Implementation implementation) : RabbitMqConsumerService<OrderHistoryServiceResponseListener>(logger, rabbitConnection)
    {
        public override string QueueName() => RabbitMqQueues.OrderHistoryResponseQueue;
        public override async Task OnMessage(string svcPayload)
        {
            await implementation.SendToUser<DataServicePayload>(svcPayload);
        }
    }
}
