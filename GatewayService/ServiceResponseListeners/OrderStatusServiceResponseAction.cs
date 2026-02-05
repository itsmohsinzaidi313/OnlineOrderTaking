using GatewayService.Interfaces;
using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Settings;

namespace GatewayService.ServiceResponseListeners
{
    public class OrderStatusServiceResponseAction(Implementation implementation) : IOrderStatusResponseAction
    {
        public string QueueName() => RabbitMqQueues.OrderStatusResponseQueue;
        public async Task OnMessage(string svcPayload)
        {
            await implementation.SendToUser<OrderStatusPayload>(svcPayload);
        }
    }
}
