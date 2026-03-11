using PointofSaleModels.Application;

namespace PointofSaleModels.ServicePayloads
{
    public class CustomerOrderHistoryServicePayload : ServicePayload
    {
        public CustomerOrderHistoryServicePayload() { }

        public CustomerOrderHistoryServicePayload(CustomerOrderHistoryServicePayload payload) : base(payload)
        {
            OrderToken = payload.OrderToken;
        }

        public string OrderToken { get; set; }
        public object? DataPayload { get; set; }
    }
}
