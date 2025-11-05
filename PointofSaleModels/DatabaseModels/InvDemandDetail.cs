namespace PointofSaleModels.DatabaseModels;

public partial class InvDemandDetail
{
    public int DemandDetailId { get; set; }

    public int DemandMasterId { get; set; }

    public int ProductDetailId { get; set; }

    public double? DemandQuantityInConsume { get; set; }

    public int? DemandUnitIdInConsume { get; set; }

    public double? IssueQuantityInConsume { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? UserIp { get; set; }

    public double? DemandQuantityInIssue { get; set; }

    public int? DemandUnitIdInIssue { get; set; }

    public virtual InvDemandMaster DemandMaster { get; set; } = null!;

    public virtual InvSetupUnit? DemandUnitIdInConsumeNavigation { get; set; }

    public virtual InvSetupUnit? DemandUnitIdInIssueNavigation { get; set; }

    public virtual ICollection<InvIssuenceDetail> InvIssuenceDetails { get; set; } = new List<InvIssuenceDetail>();

    public virtual ProductDetail ProductDetail { get; set; } = null!;
}
