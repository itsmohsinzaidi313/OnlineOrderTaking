using System;
using System.Collections.Generic;

namespace PointofSaleModels.DatabaseModels;

public partial class InvIssuanceMaster
{
    public int IssuanceMasterId { get; set; }

    public string IssuanceNumber { get; set; } = null!;

    public DateOnly? IssuanceDate { get; set; }

    public int? BranchId { get; set; }

    public int DemandMasterId { get; set; }

    public double? TotalIssuanceQuantity { get; set; }

    public bool IsActive { get; set; }

    public bool IsSubmit { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public int? UserId { get; set; }

    public string? UserIp { get; set; }

    public int CompanyId { get; set; }

    public virtual BranchMaster? Branch { get; set; }

    public virtual InvDemandMaster DemandMaster { get; set; } = null!;

    public virtual ICollection<InvIssuenceDetail> InvIssuenceDetails { get; set; } = new List<InvIssuenceDetail>();

    public virtual ICollection<InvReceivingMaster> InvReceivingMasters { get; set; } = new List<InvReceivingMaster>();
}
