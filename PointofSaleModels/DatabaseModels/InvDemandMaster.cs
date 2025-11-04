namespace PointofSaleModels.DatabaseModels;

public partial class InvDemandMaster
{
    public int DemandMasterId { get; set; }

    public string DemandNumber { get; set; } = null!;

    public DateOnly DemandDate { get; set; }

    public int BranchId { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public bool IsActive { get; set; }

    public int? StatusId { get; set; }

    public int CompanyId { get; set; }

    public string? UserIp { get; set; }

    public bool IsSubmit { get; set; }

    public virtual ICollection<InvDemandDetail> InvDemandDetails { get; set; } = new List<InvDemandDetail>();

    public virtual ICollection<InvIssuanceMaster> InvIssuanceMasters { get; set; } = new List<InvIssuanceMaster>();
}
