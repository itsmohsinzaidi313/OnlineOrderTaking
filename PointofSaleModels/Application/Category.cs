namespace PointofSaleModels.Application
{
    public class Category
    {
        public int Order { get; set; } = 0;
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public List<MenuItem> Items { get; set; } = [];
    }
}
