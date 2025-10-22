namespace PointofSaleModels.Application
{
    public class DineinTable
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public TableStatus Status { get; set; } = TableStatus.Available;
    }
}
