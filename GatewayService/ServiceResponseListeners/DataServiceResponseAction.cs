using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;

namespace GatewayService.ServiceResponseListeners
{
    public class DataServiceResponseAction(Implementation implementation) : IQueueAction
    {
        public string QueueName() => RabbitMqQueues.DataResponseQueue;
        public async Task OnMessage(string svcPayload)
        {
            await implementation.SendToUser<DataServicePayload>("DataResponse", svcPayload);
        }
    }
}
