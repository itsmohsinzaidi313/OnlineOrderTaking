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
        
    }
}
