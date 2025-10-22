using System;
using System.Collections.Generic;

namespace PointofSaleModels.DatabaseModels;

public partial class InvTransferMaster
{
    public int TransferId { get; set; }

    public string TransferNo { get; set; } = null!;

    public int BranchIdto { get; set; }

    public int BranchIdfrom { get; set; }

    public DateTime CreatedDate { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public string? UserIp { get; set; }

    public bool IsActive { get; set; }

    public DateTime Date { get; set; }

    public int? StatusId { get; set; }

    public string? ReferenceNo { get; set; }

    public bool IsSubmit { get; set; }

    public virtual BranchMaster BranchIdfromNavigation { get; set; } = null!;

    public virtual BranchMaster BranchIdtoNavigation { get; set; } = null!;

    public virtual ICollection<InvReceivingMaster> InvReceivingMasters { get; set; } = new List<InvReceivingMaster>();

    public virtual ICollection<InvTransferDetail> InvTransferDetails { get; set; } = new List<InvTransferDetail>();
}
