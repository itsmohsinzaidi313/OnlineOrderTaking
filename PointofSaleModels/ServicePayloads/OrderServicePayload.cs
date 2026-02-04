using PointofSaleModels.Application;

namespace PointofSaleModels.ServicePayloads
{
    public class OrderServicePayload : ServicePayload
    {
        public OrderServicePayload() { }
        public OrderServicePayload(ServicePayload payload) : base(payload) { }
        public CustomerOrder? Order { get; set; }
        public object? DataPayload { get; set; }
    }
}
