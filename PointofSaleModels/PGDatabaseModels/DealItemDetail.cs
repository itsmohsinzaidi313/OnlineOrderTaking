namespace PointofSaleModels.PGDatabaseModels;

public partial class DealItemDetail
{
    public int DealItemId { get; set; }

    public string? DealOptionName { get; set; }

    public int ProductDetailId { get; set; }

    public int? Quantity { get; set; }

    public bool? IsToppingAllowed { get; set; }

    public int? SizeId { get; set; }

    public bool IsActive { get; set; }

    public int? SortOrder { get; set; }

    public int? TempDealItemId { get; set; }

    public int? MaxQuantity { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ProductDetail ProductDetail { get; set; } = null!;
}
