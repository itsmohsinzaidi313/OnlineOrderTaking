namespace PointofSaleModels.PGDatabaseModels;

public partial class DiscountOrderModeMapping
{
    public int DiscountOrderModeMappingId { get; set; }

    public int DiscountId { get; set; }

    public int OrderModeId { get; set; }

    public bool IsActive { get; set; }
}
