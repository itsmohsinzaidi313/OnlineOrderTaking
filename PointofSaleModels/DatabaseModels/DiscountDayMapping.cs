namespace PointofSaleModels.DatabaseModels;

public partial class DiscountDayMapping
{
    public int DiscountDayMappingId { get; set; }

    public int DiscountId { get; set; }

    public int DayId { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public bool IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public string? UserIp { get; set; }

    public virtual UserLogin? CreatedByNavigation { get; set; }

    public virtual SetupMasterDetail Day { get; set; } = null!;

    public virtual Discount Discount { get; set; } = null!;

    public virtual UserLogin? ModifiedByNavigation { get; set; }
}
