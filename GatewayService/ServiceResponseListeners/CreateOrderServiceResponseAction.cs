using GatewayService.Interfaces;
using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Settings;

namespace GatewayService.ServiceResponseListeners
{
    public class CreateOrderServiceResponseAction(Implementation implementation) : ICreateOrderResponseAction
    {
        public string QueueName() => RabbitMqQueues.OrderResponseQueue;
        public async Task OnMessage(string svcPayload)
        {
            await implementation.SendToUser<OrderServicePayload>(svcPayload);
        }
    }
}
