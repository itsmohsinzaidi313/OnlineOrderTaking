namespace PointofSaleModels.DatabaseModels;

public partial class InvClosingMaster
{
    public int CloseId { get; set; }

    public DateTime? ClosingDate { get; set; }

    public int? BranchId { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? UserIp { get; set; }

    public bool IsSubmit { get; set; }

    public virtual BranchMaster? Branch { get; set; }

    public virtual ICollection<InvClosingDetail> InvClosingDetails { get; set; } = new List<InvClosingDetail>();
}
