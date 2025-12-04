using PointofSaleModels.ServicePayloads;

namespace GatewayService.Models
{
    public class PendingPayload<T> where T : ServicePayload
    {
        public string SignalRMethodName { get; set; } = null!;
        public T Payload { get; set; } = null!;
    }
}
