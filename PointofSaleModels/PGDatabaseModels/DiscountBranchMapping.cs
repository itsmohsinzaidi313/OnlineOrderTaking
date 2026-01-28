namespace PointofSaleModels.PGDatabaseModels;

public partial class DiscountBranchMapping
{
    public int DiscountBranchMappingId { get; set; }

    public int DiscountId { get; set; }

    public int BranchId { get; set; }

    public bool IsActive { get; set; }
}
