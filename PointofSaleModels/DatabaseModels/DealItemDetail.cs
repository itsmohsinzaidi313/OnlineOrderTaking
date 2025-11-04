namespace PointofSaleModels.DatabaseModels;

public partial class DealItemDetail
{
    public int DealItemId { get; set; }

    public string? DealOptionName { get; set; }

    public int ProductDetailId { get; set; }

    public int? Quantity { get; set; }

    public bool? IsToppingAllowed { get; set; }

    public int? SizeId { get; set; }

    public int? ProductPropertyId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public string? UserIp { get; set; }

    public bool IsActive { get; set; }

    public int? SortOrder { get; set; }

    public int? TempDealItemId { get; set; }

    public int? MaxQuantity { get; set; }

    public virtual ICollection<DealDescription> DealDescriptions { get; set; } = new List<DealDescription>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ProductDetail ProductDetail { get; set; } = null!;

    public virtual SetupMasterDetail? ProductProperty { get; set; }

    public virtual ProductSize? Size { get; set; }

    public virtual ICollection<TempOrderDetail> TempOrderDetails { get; set; } = new List<TempOrderDetail>();
}
