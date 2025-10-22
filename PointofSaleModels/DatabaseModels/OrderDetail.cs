using System;
using System.Collections.Generic;

namespace PointofSaleModels.DatabaseModels;

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

    public DateTime CreatedDate { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public string? UserIp { get; set; }

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

    public bool ItemFoc { get; set; }

    public virtual SetupMasterDetail? CommisionType { get; set; }

    public virtual DealItemDetail? DealItem { get; set; }

    public virtual Discount? Discount { get; set; }

    public virtual Gst? Gst { get; set; }

    public virtual OrderMaster OrderMaster { get; set; } = null!;

    public virtual ProductDetail ProductDetail { get; set; } = null!;

    public virtual ProductDetailProperty? ProductDetailProperty { get; set; }

    public virtual SetupMasterDetail? ProductProperty { get; set; }

    public virtual ICollection<SalesReturnDetail> SalesReturnDetails { get; set; } = new List<SalesReturnDetail>();
}
