
namespace PointofSaleModels.Application
{
    public abstract class CustomerOrderContext(bool kot = false, DateTime? orderTime = null)
    {
        public int? Id { get; set; }
        public User User { get; set; } = null!;
        public DateTime OrderTime { get; set; } = orderTime ?? DateTime.Now;
        public Rider? Rider { get; set; }
        public Customer? Customer { get; set; }
        public Discount? Discount { get; set; }
        public string Description { get; set; } = string.Empty;
        public ExtraCharge? DeliveryCharges { get; set; }
    }
}
