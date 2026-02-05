using PointofSaleModels.Application;

namespace PointofSaleModels.ServicePayloads
{
    public class OrderServicePayload : ServicePayload
    {
        public OrderServicePayload() { }
        public OrderServicePayload(OrderServicePayload payload) : base(payload)
        {
            Order = payload.Order;
        }
        public CustomerOrder? Order { get; set; }
        public object? DataPayload { get; set; }
    }
}
