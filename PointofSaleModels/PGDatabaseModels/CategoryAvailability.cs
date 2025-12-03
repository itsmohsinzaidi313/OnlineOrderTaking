namespace PointofSaleModels.PGDatabaseModels;

public partial class CategoryAvailability
{
    public int CategoryAvailableId { get; set; }

    public int? CategoryId { get; set; }

    public int? DayId { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public bool? IsActive { get; set; }

    public virtual ProductCategory? Category { get; set; }

    public virtual SetupMasterDetail? Day { get; set; }
}
