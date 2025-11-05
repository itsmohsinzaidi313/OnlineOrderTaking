namespace PointofSaleModels.DatabaseModels;

public partial class BusinessDay
{
    public int BusinessDayId { get; set; }

    public DateTime? Date { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? UserIp { get; set; }

    public int? BranchId { get; set; }

    public bool IsDayOpen { get; set; }

    public DateTime? OpenDate { get; set; }

    public DateTime? CloseDate { get; set; }

    public int? OpenedBy { get; set; }

    public int? ClosedBy { get; set; }

    public virtual BranchMaster? Branch { get; set; }

    public virtual ICollection<ShiftDetail> ShiftDetails { get; set; } = new List<ShiftDetail>();

    public virtual ICollection<TempOrderMaster> TempOrderMasters { get; set; } = new List<TempOrderMaster>();

    public virtual ICollection<TerminalDetail> TerminalDetails { get; set; } = new List<TerminalDetail>();
}
