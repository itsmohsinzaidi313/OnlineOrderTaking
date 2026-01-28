namespace PointofSaleModels.PGDatabaseModels;

public partial class DiscountProductDetailMapping
{
    public int DiscountProductDetailMappingId { get; set; }

    public int DiscountId { get; set; }

    public int ProductDetailId { get; set; }

    public bool IsActive { get; set; }
}
