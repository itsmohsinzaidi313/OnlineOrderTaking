namespace ImportService.Entities
{
    public class DealDescription
    {
        public int DealDescId { get; set; }

        public int? DealItemId { get; set; }

        public int? ProductDetailId { get; set; }

        public bool? IsActive { get; set; }

        public int? SortOrder { get; set; }

        public double? Price { get; set; }
        public DealDescription CopyWith(DealDescription instance)
        {
            return new DealDescription
            {
                DealDescId = instance.DealDescId,
                DealItemId = instance.DealItemId,
                ProductDetailId = instance.ProductDetailId,
                IsActive = instance.IsActive,
                SortOrder = instance.SortOrder,
                Price = instance.Price
            };
        }
    }
}
