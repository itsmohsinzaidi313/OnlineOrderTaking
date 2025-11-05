namespace PointofSaleModels.DatabaseModels;

public partial class Terminal
{
    public int TerminalId { get; set; }

    public string? TerminalName { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? CompanyId { get; set; }

    public string? UserIp { get; set; }

    public int? BranchId { get; set; }

    public Guid? UniqueId { get; set; }

    public virtual BranchMaster? Branch { get; set; }

    public virtual SetupCompany? Company { get; set; }

    public virtual ICollection<TerminalDetail> TerminalDetails { get; set; } = new List<TerminalDetail>();
}
