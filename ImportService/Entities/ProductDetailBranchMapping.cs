namespace ImportService.Entities
{
    public class ProductDetailBranchMapping
    {
        public int ProductDetailBranchMappingId { get; set; }

        public int? ProductDetailId { get; set; }

        public int? BranchId { get; set; }

        public bool? IsActive { get; set; }

        public bool IsDayWise { get; set; }

        public bool IsEnable { get; set; }

        public string? RemoteId { get; set; }
        public ProductDetailBranchMapping CopyWith(ProductDetailBranchMapping instance)
        {
            return new ProductDetailBranchMapping
            {
                ProductDetailBranchMappingId = instance.ProductDetailBranchMappingId,
                ProductDetailId = instance.ProductDetailId,
                BranchId = instance.BranchId,
                IsActive = instance.IsActive,
                IsDayWise = instance.IsDayWise,
                IsEnable = instance.IsEnable,
                RemoteId = instance.RemoteId
            };
        }
    }
}
