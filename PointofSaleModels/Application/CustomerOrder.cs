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
        public List<MenuItem>? ItemUpdate { get; set; } = [];
        public List<MenuItem>? AddItems { get; set; } = [];
        public List<MenuItem>? LessItems { get; set; } = [];
        public List<PaymentMethod>? PaymentMethods { get; set; }
        public bool Merged { get; set; }
        public Tax? Tax { get; set; }

        public CustomerOrder() : base()
        {
        }
    }
}
