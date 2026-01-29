namespace ImportService.Entities
{
    public class DiscountOrderTypeMapping
    {
        public int DiscountOrderTypeMappingId { get; set; }

        public int DiscountId { get; set; }

        public int OrderTypeId { get; set; }

        public bool IsActive { get; set; }
        public DiscountOrderTypeMapping CopyWith(DiscountOrderTypeMapping instance)
        {
            return new DiscountOrderTypeMapping
            {
                DiscountOrderTypeMappingId = instance.DiscountOrderTypeMappingId,
                DiscountId = instance.DiscountId,
                OrderTypeId = instance.OrderTypeId,
                IsActive = instance.IsActive
            };
        }
    }
}
