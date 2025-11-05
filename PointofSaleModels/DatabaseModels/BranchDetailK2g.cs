namespace PointofSaleModels.DatabaseModels;

public partial class BranchDetailK2g
{
    public int? BranchDetailId { get; set; }

    public int BranchId { get; set; }

    public int AreaId { get; set; }

    public int? DeliveryTime { get; set; }

    public double? MinimumOrder { get; set; }

    public double? DeliveryCharges { get; set; }

    public int? AlternateBranch1 { get; set; }

    public int? AlternateBranch2 { get; set; }

    public int? AlternateBranch3 { get; set; }

    public int? DeliveryTime1 { get; set; }

    public int? DeliveryTime2 { get; set; }

    public int? DeliveryTime3 { get; set; }

    public DateTime CreatedDate { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public string? UserIp { get; set; }

    public bool IsActive { get; set; }

    public bool? IsEnabled { get; set; }

    public int ExtraDeliveryTime { get; set; }
}
