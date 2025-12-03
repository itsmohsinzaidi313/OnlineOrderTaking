namespace PointofSaleModels.PGDatabaseModels;

public partial class DiscountDayMapping
{
    public int DiscountDayMappingId { get; set; }

    public int DiscountId { get; set; }

    public int DayId { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public bool IsActive { get; set; }

    public virtual Discount Discount { get; set; } = null!;
}
