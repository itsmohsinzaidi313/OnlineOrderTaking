namespace PointofSaleModels.DatabaseModels;

public partial class InvRequisitionDetail
{
    public int RequisitionDetailId { get; set; }

    public int? RequisitionId { get; set; }

    public int? ProductDetailId { get; set; }

    public double? QuantityInPurchase { get; set; }

    public int? PurchaseUnitId { get; set; }

    public double? QuantityInIssue { get; set; }

    public int? IssueUnitId { get; set; }

    public double? QuantityInConsume { get; set; }

    public int? ConsumeUnitId { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? UserIp { get; set; }

    public double TotalPoquantityInConsume { get; set; }

    public virtual InvSetupUnit? ConsumeUnit { get; set; }

    public virtual ICollection<InvPodetail> InvPodetails { get; set; } = new List<InvPodetail>();

    public virtual InvSetupUnit? IssueUnit { get; set; }

    public virtual ProductDetail? ProductDetail { get; set; }

    public virtual InvSetupUnit? PurchaseUnit { get; set; }

    public virtual InvRequisitionMaster? Requisition { get; set; }
}
