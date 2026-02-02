namespace ExportService.Entities
{
    public class DiscountProductDetailMapping
    {
        public int DiscountProductDetailMappingId { get; set; }

        public int DiscountId { get; set; }

        public int ProductDetailId { get; set; }

        public bool IsActive { get; set; }
        public DiscountProductDetailMapping CopyWith(DiscountProductDetailMapping instance)
        {
            return new DiscountProductDetailMapping
            {
                DiscountProductDetailMappingId = instance.DiscountProductDetailMappingId,
                DiscountId = instance.DiscountId,
                ProductDetailId = instance.ProductDetailId,
                IsActive = instance.IsActive
            };
        }
    }
}
