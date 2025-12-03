using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;

namespace GatewayService.ServiceResponseListeners
{
    public class CreateOrderServiceResponseAction(Implementation implementation) : IQueueAction
    {
        public string QueueName() => RabbitMqQueues.OrderResponseQueue;
        public async Task OnMessage(ServicePayload svcPayload)
        {
            await implementation.ExecuteHandler(QueueName(), svcPayload);
        }
    }
}
