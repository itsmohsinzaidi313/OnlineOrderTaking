namespace PointofSaleModels.Application
{
    public class ExtraCharge
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Value { get; set; }
        public OrderType OrderType { get; set; }
        public ValueType Type { get; set; } = ValueType.Amount;
    }
}
