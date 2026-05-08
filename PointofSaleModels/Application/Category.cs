namespace PointofSaleModels.Application
{
    public class Category
    {
        public int SortOrder { get; set; } = 0;
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public List<MenuItem> Items { get; set; } = [];
        public string Layout { get; set; }
    }
}
