using System;
using System.Collections.Generic;

namespace PointofSaleModels.DatabaseModels;

public partial class InvRequisitionMaster
{
    public int RequisitionId { get; set; }

    public string RequisitionNumber { get; set; } = null!;

    public int? StatusId { get; set; }

    public int? UserId { get; set; }

    public int? BranchId { get; set; }

    public DateTime Date { get; set; }

    public bool IsApprove { get; set; }

    public DateTime CreatedDate { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public string? UserIp { get; set; }

    public bool IsActive { get; set; }

    public bool IsSubmit { get; set; }

    public virtual BranchMaster? Branch { get; set; }

    public virtual UserLogin CreatedByNavigation { get; set; } = null!;

    public virtual ICollection<InvRequisitionDetail> InvRequisitionDetails { get; set; } = new List<InvRequisitionDetail>();

    public virtual UserLogin? ModifiedByNavigation { get; set; }

    public virtual SetupMasterDetail? Status { get; set; }

    public virtual UserLogin? User { get; set; }
}
