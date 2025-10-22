namespace PointofSaleModels.Application
{
    public class PaymentMethod
    {
        public PaymentType Type { get; set; } = PaymentType.Cash;
        public double Amount { get; set; } = 0.0;
        public string Description { get; set; } = string.Empty;
        public double Change { get; set; } = 0.0;
        public double Received { get; set; } = 0.0;
    }
}
