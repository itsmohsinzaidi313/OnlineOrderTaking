namespace PointofSaleModels.Application
{
    public class MenuItem
    {
        public const long KOpenFoodCode = 151605140604;
        public int Id { get; set; }
        public string CategoryId { get; set; } = "0";
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public double Price { get; set; } = 0.0;
        public double TaxAmount { get; set; } = 0.0;
        public double Quantity { get; set; } = 0.0;
        public string Image { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public bool IsKot { get; set; } = false;
        public bool ItemFOC { get; set; }
        public List<ItemVariation> Variations { get; set; } = [];
    }
}
