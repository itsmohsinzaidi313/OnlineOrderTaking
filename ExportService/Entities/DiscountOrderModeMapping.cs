namespace ExportService.Entities
{
    public class DiscountOrderModeMapping
    {
        public int DiscountOrderModeMappingId { get; set; }

        public int DiscountId { get; set; }

        public int OrderModeId { get; set; }

        public bool IsActive { get; set; }
        public DiscountOrderModeMapping CopyWith(DiscountOrderModeMapping instance)
        {
            return new DiscountOrderModeMapping
            {
                DiscountOrderModeMappingId = instance.DiscountOrderModeMappingId,
                DiscountId = instance.DiscountId,
                OrderModeId = instance.OrderModeId,
                IsActive = instance.IsActive
            };
        }
    }
}
