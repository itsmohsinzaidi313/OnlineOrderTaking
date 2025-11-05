namespace PointofSaleModels.DatabaseModels;

public partial class DiscountAreaMapping
{
    public int DiscountAreaMappingId { get; set; }

    public int DiscountId { get; set; }

    public int AreaId { get; set; }

    public bool IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public string? UserIp { get; set; }

    public virtual Area Area { get; set; } = null!;

    public virtual UserLogin? CreatedByNavigation { get; set; }

    public virtual Discount Discount { get; set; } = null!;

    public virtual UserLogin? ModifiedByNavigation { get; set; }
}
