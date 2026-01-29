using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;

namespace GatewayService.ServiceResponseListeners
{
    public class ImportServiceResponseAction(Implementation implementation) : IQueueAction
    {
        public string QueueName() => RabbitMqQueues.ImportResponseQueue;

        public async Task OnMessage(string svcPayload)
        {
            await implementation.SendToUser<ImportServicePayload>(svcPayload);
        }
    }
}
