using System;
using System.Collections.Generic;

namespace PointofSaleModels.DatabaseModels;

public partial class InvPodetail
{
    public int PodetailId { get; set; }

    public int? Poid { get; set; }

    public int? ProductDetailId { get; set; }

    public int? RequisitionDetailId { get; set; }

    public double PurchaseUnitPrice { get; set; }

    public double SubTotal { get; set; }

    public double TaxAmount { get; set; }

    public double Discount { get; set; }

    public double NetAmount { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? UserIp { get; set; }

    public int? BatchId { get; set; }

    public double? PurchaseQuantity { get; set; }

    public double? IssueQuantity { get; set; }

    public double? ConsumeQuantity { get; set; }

    public int? PurchaseUnitId { get; set; }

    public int? IssueUnitId { get; set; }

    public int? ConsumeUnitId { get; set; }

    public double? GrnremainingQuantity { get; set; }

    public virtual InvBatch? Batch { get; set; }

    public virtual InvSetupUnit? ConsumeUnit { get; set; }

    public virtual InvSetupUnit? IssueUnit { get; set; }

    public virtual InvPomaster? Po { get; set; }

    public virtual ProductDetail? ProductDetail { get; set; }

    public virtual InvSetupUnit? PurchaseUnit { get; set; }

    public virtual InvRequisitionDetail? RequisitionDetail { get; set; }
}
