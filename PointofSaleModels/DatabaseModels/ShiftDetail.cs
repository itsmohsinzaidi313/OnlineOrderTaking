using System;
using System.Collections.Generic;

namespace PointofSaleModels.DatabaseModels;

public partial class ShiftDetail
{
    public int ShiftDetailId { get; set; }

    public int? ShiftId { get; set; }

    public int? BusinessDayId { get; set; }

    public string? ShiftNum { get; set; }

    public DateTime? OpeningDate { get; set; }

    public int? ShiftOpenUserId { get; set; }

    public DateTime? ClosingDate { get; set; }

    public int? ShiftCloseUserId { get; set; }

    public bool IsShiftOpen { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? BranchId { get; set; }

    public virtual BranchMaster? Branch { get; set; }

    public virtual BusinessDay? BusinessDay { get; set; }

    public virtual ICollection<OrderMaster> OrderMasters { get; set; } = new List<OrderMaster>();

    public virtual Shift? Shift { get; set; }

    public virtual ICollection<TempOrderMaster> TempOrderMasters { get; set; } = new List<TempOrderMaster>();

    public virtual ICollection<TerminalDetail> TerminalDetails { get; set; } = new List<TerminalDetail>();
}
