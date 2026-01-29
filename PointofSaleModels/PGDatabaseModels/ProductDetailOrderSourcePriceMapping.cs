namespace PointofSaleModels.PGDatabaseModels;

public partial class ProductDetailOrderSourcePriceMapping
{
    public int MapId { get; set; }

    public int? OrderSourceId { get; set; }

    public int? ProductDetailId { get; set; }

    public double Price { get; set; }

    public bool IsActive { get; set; }

    public double? FuturePrice { get; set; }

    public double? PreviousPrice { get; set; }

    public int? BranchId { get; set; }
}
