using PointofSaleModels.ServicePayloads;

namespace PointofSaleModels.Services
{
    public interface IQueueAction
    {
        public string QueueName();
        public Task OnMessage(ServicePayload transport);
    }
}
