using PointofSaleModels.Application;

namespace PointofSaleModels
{
    public abstract class CustomerOrderContext(bool kot = false, DateTime? orderTime = null)
    {
        public int? Id { get; set; }
        public User User { get; set; } = null!;
        public DateTime OrderTime { get; set; } = orderTime ?? DateTime.Now;
        public bool Kot { get; set; } = kot;
        public Waiter? Waiter { get; set; }
        public DineinTable? Table { get; set; }
        public int? Persons { get; set; }
        public Rider? Rider { get; set; }
        public Customer? Customer { get; set; }
        public Car? Car { get; set; }
        public Discount? Discount { get; set; }
        public string Description { get; set; } = string.Empty;

        public ExtraCharge? DeliveryCharges { get; set; }
        public ExtraCharge? ServiceCharges { get; set; }
    }
}
