namespace PointofSaleModels.Application
{
    public class CustomerOrder : CustomerOrderContext
    {
        public string OrderNumber { get; set; } = string.Empty;
        public OrderType OrderType { get; set; }
        public PaymentType? PaymentType { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }
        public OrderStatus? Status { get; set; }
        public List<MenuItem> Items { get; set; } = [];
        public CustomerOrder() : base()
        {
        }
    }
}
