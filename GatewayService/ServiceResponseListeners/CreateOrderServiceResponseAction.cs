using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;

namespace GatewayService.ServiceResponseListeners
{
    public class CreateOrderServiceResponseAction(Implementation implementation) : IQueueAction
    {
        public string QueueName() => RabbitMqQueues.OrderResponseQueue;
        public async Task OnMessage(string svcPayload)
        {
            var payload = System.Text.Json.JsonSerializer.Deserialize<CreateOrderServicePayload>(svcPayload);
            await implementation.ExecuteHandler(QueueName(), payload);
        }
    }
}
