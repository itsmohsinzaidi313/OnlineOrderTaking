namespace PointofSaleModels.Application
{
    public class Terminal
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid UniqueId { get; set; }
        public bool IsOpen { get; set; }
    }
}
