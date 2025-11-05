namespace PointofSaleModels.DatabaseModels;

public partial class TempOrderMaster
{
    public int TempOrderMasterId { get; set; }

    public int? CompanyId { get; set; }

    public string? OrderNumber { get; set; }

    public int? BranchId { get; set; }

    public DateTime? OrderDate { get; set; }

    public TimeOnly? OrderTime { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? UserIp { get; set; }

    public bool IsUse { get; set; }

    public int? BusinessDayId { get; set; }

    public int? ShiftDetailId { get; set; }

    public int? TerminalDetailId { get; set; }

    public virtual BranchMaster? Branch { get; set; }

    public virtual BusinessDay? BusinessDay { get; set; }

    public virtual SetupCompany? Company { get; set; }

    public virtual ShiftDetail? ShiftDetail { get; set; }

    public virtual ICollection<TempOrderDetail> TempOrderDetails { get; set; } = new List<TempOrderDetail>();

    public virtual TerminalDetail? TerminalDetail { get; set; }
}
