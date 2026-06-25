using PointofSaleModels.ServicePayloads;

namespace ClientResponseService.Models
{
    public class PendingPayload<T> where T : ServicePayload
    {
        public T Payload { get; set; } = null!;
    }
}
