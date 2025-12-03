using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;

namespace GatewayService.ServiceResponseListeners
{
    public class MenuServiceResponseAction(Implementation implementation) : IQueueAction
    {
        public string QueueName() => RabbitMqQueues.MenuResponseQueue;
        public async Task OnMessage(ServicePayload transport)
        {
            await implementation.ExecuteHandler(QueueName(), transport);
        }
    }
}
