namespace PointofSaleModels.Entities
{
    public class ProductSize
    {
        public int SizeId { get; set; }

        public string SizeName { get; set; }

        public int? CompanyId { get; set; }
        
        public bool IsActive { get; set; }
        
    }
}
