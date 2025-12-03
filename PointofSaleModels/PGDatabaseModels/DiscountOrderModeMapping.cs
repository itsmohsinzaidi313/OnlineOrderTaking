namespace PointofSaleModels.PGDatabaseModels;

public partial class DiscountOrderModeMapping
{
    public int DiscountOrderModeMappingId { get; set; }

    public int DiscountId { get; set; }

    public int OrderModeId { get; set; }

    public bool IsActive { get; set; }

    public virtual Discount Discount { get; set; } = null!;
}
