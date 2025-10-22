namespace PointofSaleModels.Application
{
    public class Discount
    {
        public int Id { get; set; }
        public ValueType Type { get; set; } = ValueType.Amount;
        public string Name { get; set; } = string.Empty;
        public double Value { get; set; } = 0.0;
        public double MaxCap { get; set; }
        public double MinCap { get; set; }
    }
}
