namespace PointofSaleModels.DatabaseModels;

public partial class InvConsumptionMaster
{
    public int ConsumptionId { get; set; }

    public DateTime? Date { get; set; }

    public int? RecipeId { get; set; }

    public int? BranchId { get; set; }

    public double? TotalQty { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? UserIp { get; set; }

    public bool IsActive { get; set; }

    public int? OrderMasterId { get; set; }

    public int? ProductionId { get; set; }

    public int? EmployeeMealMasterId { get; set; }

    public bool IsRefund { get; set; }

    public double RefundQty { get; set; }

    public virtual BranchMaster? Branch { get; set; }

    public virtual InvEmployeeMealMaster? EmployeeMealMaster { get; set; }

    public virtual ICollection<InvConsumptionBatchDetail> InvConsumptionBatchDetails { get; set; } = new List<InvConsumptionBatchDetail>();

    public virtual ICollection<InvConsumptionDetail> InvConsumptionDetails { get; set; } = new List<InvConsumptionDetail>();

    public virtual InvSubRecipeProductionMaster? Production { get; set; }

    public virtual InvRecipeMaster? Recipe { get; set; }
}
