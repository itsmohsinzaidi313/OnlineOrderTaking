namespace PointofSaleModels.DatabaseModels;

public partial class TerminalDetail
{
    public int TerminalDetailId { get; set; }

    public int? TerminalId { get; set; }

    public int? ShiftDetailId { get; set; }

    public int? BusinessDayId { get; set; }

    public DateTime? OpeningDate { get; set; }

    public int? TerminalOpenUserId { get; set; }

    public bool IsTerminalOpen { get; set; }

    public double? TerminalOpeningAmount { get; set; }

    public DateTime? ClosingDate { get; set; }

    public int? TerminalCloseUserId { get; set; }

    public double? TerminalClosingAmount { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? UserIp { get; set; }

    public int? BranchId { get; set; }

    public string? UniqueId { get; set; }

    public virtual BranchMaster? Branch { get; set; }

    public virtual BusinessDay? BusinessDay { get; set; }

    public virtual ICollection<OrderPayment> OrderPayments { get; set; } = new List<OrderPayment>();

    public virtual ShiftDetail? ShiftDetail { get; set; }

    public virtual ICollection<TempOrderMaster> TempOrderMasters { get; set; } = new List<TempOrderMaster>();

    public virtual Terminal? Terminal { get; set; }
}
