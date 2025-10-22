namespace PointofSaleModels
{
    public class Tax
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Value { get; set; } = 0.0;
        public PaymentType PaymentType { get; set; }
    }
}
