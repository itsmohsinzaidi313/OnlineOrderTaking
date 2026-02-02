namespace PointofSaleModels.Application
{
    public class CustomerOrder : CustomerOrderContext
    {
        public string Domain { get; set; }
        public int BranchId { get; set; }
        public CustomerDetail CustomerDetails { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public OrderType? OrderType { get; set; }
        public PaymentType? PaymentType { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }
        public OrderStatus? Status { get; set; }
        public List<MenuItem> Items { get; set; } = [];
        public CustomerOrder() : base()
        {
        }
    }

    public sealed class CustomerDetail
    {
        public string? Title { get; set; }

        public string? FullName { get; set; }

        public string? MobileNumber { get; set; }

        public string? AlternateMobileNumber { get; set; }

        public string? DeliveryAddress { get; set; }

        public string? NearestLandmark { get; set; }

        public string? EmailAddress { get; set; }

        public string? DeliveryInstructions { get; set; }

        public string? PaymentMethod { get; set; }

        public bool IsGift { get; set; }

        public string? RecipientName { get; set; }

        public string? RecipientNumber { get; set; }

        public string? GiftingMessage { get; set; }
    }
}
