namespace ImportService.Entities
{
    public class DiscountBranchMapping
    {
        public int DiscountBranchMappingId { get; set; }

        public int DiscountId { get; set; }

        public int BranchId { get; set; }

        public bool IsActive { get; set; }
        public DiscountBranchMapping CopyWith(DiscountBranchMapping instance)
        {
            return new DiscountBranchMapping
            {
                DiscountBranchMappingId = instance.DiscountBranchMappingId,
                DiscountId = instance.DiscountId,
                BranchId = instance.BranchId,
                IsActive = instance.IsActive
            };
        }
    }
}
