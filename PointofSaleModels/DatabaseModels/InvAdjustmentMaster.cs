namespace PointofSaleModels.DatabaseModels;

public partial class InvAdjustmentMaster
{
    public int InvAdjustmentId { get; set; }

    public string AdjustmentNo { get; set; } = null!;

    public DateTime InvAdjustmentDate { get; set; }

    public DateTime CreatedDate { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public string? UserIp { get; set; }

    public bool IsActive { get; set; }

    public int? BranchId { get; set; }

    public bool IsSubmit { get; set; }

    public virtual BranchMaster? Branch { get; set; }

    public virtual UserLogin CreatedByNavigation { get; set; } = null!;

    public virtual ICollection<InvAdjustmentDetail> InvAdjustmentDetails { get; set; } = new List<InvAdjustmentDetail>();

    public virtual UserLogin? ModifiedByNavigation { get; set; }
}
