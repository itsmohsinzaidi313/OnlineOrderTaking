using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;

namespace GatewayService.ServiceResponseListeners
{
    public class LoginServiceResponseAction(Implementation implementation) : IQueueAction
    {
        public string QueueName() => RabbitMqQueues.LoginResponseQueue;
        public async Task OnMessage(string svcPayload)
        {
            await implementation.ExecuteHandler(QueueName(), svcPayload);
        }
    }
}
