namespace PointofSaleModels.ServicePayloads
{
    public class OrderStatusPayload : ServicePayload
    {
        public OrderStatusPayload() : base()
        {
        }

        public OrderStatusPayload(OrderStatusPayload payload) : base(payload)
        {
            OrderNumber = payload.OrderNumber;
            OrderStatusId = payload.OrderStatusId;
            BranchTransferId = payload.BranchTransferId;
        }
        public string OrderNumber { get; set; }
        public int? OrderStatusId { get; set; }
        public int? BranchTransferId { get; set; }
        public int? RiderId { get; set; }
        public object? DataPayload { get; set; }
    }
}
