using PointofSaleModels.ServicePayloads;

namespace GatewayService.Models
{
    public class PendingPayload<T> where T : ServicePayload
    {
        public T Payload { get; set; } = null!;
    }
}
