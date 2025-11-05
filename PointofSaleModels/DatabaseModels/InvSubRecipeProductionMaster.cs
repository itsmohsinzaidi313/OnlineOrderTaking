namespace PointofSaleModels.DatabaseModels;

public partial class InvSubRecipeProductionMaster
{
    public int ProductionId { get; set; }

    public string? ProductionNumber { get; set; }

    public int? UserId { get; set; }

    public DateTime? Date { get; set; }

    public int? BranchId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public string? UserIp { get; set; }

    public bool? IsActive { get; set; }

    public bool IsSubmit { get; set; }

    public virtual BranchMaster? Branch { get; set; }

    public virtual UserLogin? CreatedByNavigation { get; set; }

    public virtual ICollection<InvConsumptionMaster> InvConsumptionMasters { get; set; } = new List<InvConsumptionMaster>();

    public virtual ICollection<InvSubRecipeProductionDetail> InvSubRecipeProductionDetails { get; set; } = new List<InvSubRecipeProductionDetail>();

    public virtual UserLogin? ModifiedByNavigation { get; set; }

    public virtual UserLogin? User { get; set; }
}
