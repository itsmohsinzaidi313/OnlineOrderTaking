namespace PointofSaleModels.ServicePayloads
{
    public class OrderUpdatePayload : ServicePayload
    {
        public OrderUpdatePayload() : base()
        {
        }

        public OrderUpdatePayload(OrderUpdatePayload payload) : base(payload)
        {
            OrderToken = payload.OrderToken;
            OrderStatusId = payload.OrderStatusId;
            BranchTransferId = payload.BranchTransferId;
            RiderId = payload.RiderId;
            DeliveryTime = payload.DeliveryTime;
        }
        public string OrderToken { get; set; }
        public int? OrderStatusId { get; set; }
        public int? BranchTransferId { get; set; }
        public int? RiderId { get; set; }
        public int? DeliveryTime { get; set; }
        public object? DataPayload { get; set; }
    }
}
