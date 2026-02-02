namespace PointofSaleModels.PGDatabaseModels;

public partial class OrderDetail
{
    public int OrderDetailId { get; set; }

    public int OrderMasterId { get; set; }

    public int ProductDetailId { get; set; }

    public double? Quantity { get; set; }

    public double? PriceWithoutGst { get; set; }

    public int? Gstid { get; set; }

    public double? PriceWithGst { get; set; }

    public int? OrderParentId { get; set; }

    public string? SpecialInstruction { get; set; }

    public int? DealItemId { get; set; }

    public double? DiscountPercent { get; set; }

    public bool IsActive { get; set; }

    public int? RandomId { get; set; }

    public bool IsKot { get; set; }

    public double KitchenQuantity { get; set; }

    public double LastKitchenQuantity { get; set; }

    public bool IsTopping { get; set; }

    public int? ProductDetailPropertyId { get; set; }

    public int? CommisionTypeId { get; set; }

    public double? CommisionAmount { get; set; }

    public int? ProductPropertyId { get; set; }

    public bool? IsPercentage { get; set; }

    public int? DiscountId { get; set; }

    public OrderMaster OrderMaster { get; set; } = null!;
}
