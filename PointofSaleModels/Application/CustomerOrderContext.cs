
namespace PointofSaleModels.Application
{
    public abstract class CustomerOrderContext(DateTime? orderTime = null)
    {
        public User User { get; set; } = null!;
        public DateTime OrderTime { get; set; } = orderTime ?? DateTime.Now;
        public Rider? Rider { get; set; }
        public CustomerDetail CustomerDetails { get; set; }
        public int? DeliveryCharges { get; set; }
    }
}
