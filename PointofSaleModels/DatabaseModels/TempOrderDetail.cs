using System;
using System.Collections.Generic;

namespace PointofSaleModels.DatabaseModels;

public partial class TempOrderDetail
{
    public int TempOrderDetailId { get; set; }

    public int? TempOrderMasterId { get; set; }

    public int? ProductDetailId { get; set; }

    public double? Quantity { get; set; }

    public int? TempOrderParentId { get; set; }

    public int? DealItemId { get; set; }

    public string? SpecialInstruction { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? UserIp { get; set; }

    public virtual DealItemDetail? DealItem { get; set; }

    public virtual ICollection<TempOrderDetail> InverseTempOrderParent { get; set; } = new List<TempOrderDetail>();

    public virtual ProductDetail? ProductDetail { get; set; }

    public virtual TempOrderMaster? TempOrderMaster { get; set; }

    public virtual TempOrderDetail? TempOrderParent { get; set; }
}
