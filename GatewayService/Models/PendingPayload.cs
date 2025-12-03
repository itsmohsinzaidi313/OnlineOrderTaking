using PointofSaleModels.ServicePayloads;

namespace GatewayService.Models
{
    public class PendingPayload
    {
        public string SignalRMethodName { get; set; } = null!;
        public ServicePayload Payload { get; set; } = null!;
    }
}
