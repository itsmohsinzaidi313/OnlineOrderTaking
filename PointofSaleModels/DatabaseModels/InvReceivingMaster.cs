using System;
using System.Collections.Generic;

namespace PointofSaleModels.DatabaseModels;

public partial class InvReceivingMaster
{
    public int ReceivingId { get; set; }

    public string? ReceivingNo { get; set; }

    public int? BranchId { get; set; }

    public int? UserId { get; set; }

    public int? TransferId { get; set; }

    public int? IssuanceId { get; set; }

    public DateTime? Date { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? UserIp { get; set; }

    public bool? IsSubmit { get; set; }

    public string? Comments { get; set; }

    public virtual BranchMaster? Branch { get; set; }

    public virtual UserLogin? CreatedByNavigation { get; set; }

    public virtual ICollection<InvReceivingDetail> InvReceivingDetails { get; set; } = new List<InvReceivingDetail>();

    public virtual InvIssuanceMaster? Issuance { get; set; }

    public virtual UserLogin? ModifiedByNavigation { get; set; }

    public virtual InvTransferMaster? Transfer { get; set; }

    public virtual UserLogin? User { get; set; }
}
