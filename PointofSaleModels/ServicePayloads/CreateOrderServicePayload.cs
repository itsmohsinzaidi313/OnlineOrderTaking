using PointofSaleModels.Application;

namespace PointofSaleModels.ServicePayloads
{
    public class CreateOrderServicePayload : ServicePayload
    {
        public CreateOrderServicePayload() { }
        public CreateOrderServicePayload(ServicePayload payload) : base(payload) { }
        public CustomerOrder? Order { get; set; }
    }
}
