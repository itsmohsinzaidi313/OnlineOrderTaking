namespace PointofSaleModels.Application
{
    public class ItemChoice
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public int MaxChoice { get; set; }
        public List<ItemOption> ItemOptions { get; set; } = [];
    }
}
