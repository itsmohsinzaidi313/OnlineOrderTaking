namespace PointofSaleModels.PGDatabaseModels;

public partial class DiscountDayMapping
{
    public int DiscountDayMappingId { get; set; }

    public int DiscountId { get; set; }

    public int DayId { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    public bool IsActive { get; set; }
}
