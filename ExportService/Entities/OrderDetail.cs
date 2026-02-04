namespace ExportService.Entities;

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
    public OrderDetail CopyWith(OrderDetail instance)
    {
        return new OrderDetail
        {
            OrderDetailId = instance.OrderDetailId,
            OrderMasterId = instance.OrderMasterId,
            ProductDetailId = instance.ProductDetailId,
            Quantity = instance.Quantity,
            PriceWithoutGst = instance.PriceWithoutGst,
            Gstid = instance.Gstid,
            PriceWithGst = instance.PriceWithGst,
            OrderParentId = instance.OrderParentId,
            SpecialInstruction = instance.SpecialInstruction,
            DealItemId = instance.DealItemId,
            DiscountPercent = instance.DiscountPercent,
            IsActive = instance.IsActive,
            RandomId = instance.RandomId,
            IsKot = instance.IsKot,
            KitchenQuantity = instance.KitchenQuantity,
            LastKitchenQuantity = instance.LastKitchenQuantity,
            IsTopping = instance.IsTopping,
            ProductDetailPropertyId = instance.ProductDetailPropertyId,
            CommisionTypeId = instance.CommisionTypeId,
            CommisionAmount = instance.CommisionAmount,
            ProductPropertyId = instance.ProductPropertyId,
            IsPercentage = instance.IsPercentage,
            DiscountId = instance.DiscountId
        };
    }
}
