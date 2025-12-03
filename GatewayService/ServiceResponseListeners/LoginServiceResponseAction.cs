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
            var payload = System.Text.Json.JsonSerializer.Deserialize<LoginServicePayload>(svcPayload);
            await implementation.ExecuteHandler(QueueName(), payload);
        }
    }
}
