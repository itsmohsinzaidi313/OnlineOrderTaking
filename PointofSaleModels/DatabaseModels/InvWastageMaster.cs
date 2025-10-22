using System;
using System.Collections.Generic;

namespace PointofSaleModels.DatabaseModels;

public partial class InvWastageMaster
{
    public int WastageId { get; set; }

    public string? WastageNumber { get; set; }

    public int? UserId { get; set; }

    public DateOnly? Date { get; set; }

    public int? BranchId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public string? UserIp { get; set; }

    public bool IsActive { get; set; }

    public int? StatusId { get; set; }

    public bool IsApprove { get; set; }

    public bool IsSubmit { get; set; }

    public virtual BranchMaster? Branch { get; set; }

    public virtual UserLogin? CreatedByNavigation { get; set; }

    public virtual ICollection<InvWastageDetail> InvWastageDetails { get; set; } = new List<InvWastageDetail>();

    public virtual UserLogin? ModifiedByNavigation { get; set; }

    public virtual UserLogin? User { get; set; }
}
