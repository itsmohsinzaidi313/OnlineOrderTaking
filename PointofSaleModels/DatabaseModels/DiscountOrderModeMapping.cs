namespace PointofSaleModels.DatabaseModels;

public partial class DiscountOrderModeMapping
{
    public int DiscountOrderModeMappingId { get; set; }

    public int DiscountId { get; set; }

    public int OrderModeId { get; set; }

    public bool IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public string? UserIp { get; set; }

    public virtual UserLogin? CreatedByNavigation { get; set; }

    public virtual UserLogin? ModifiedByNavigation { get; set; }

    public virtual SetupMasterDetail OrderMode { get; set; } = null!;
}
