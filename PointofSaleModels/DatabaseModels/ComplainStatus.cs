using System;
using System.Collections.Generic;

namespace PointofSaleModels.DatabaseModels;

public partial class ComplainStatus
{
    public int ComplainStatusId { get; set; }

    public string ComplainStatusName { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public string? UserIp { get; set; }

    public bool IsActive { get; set; }

    public bool? IsInitial { get; set; }

    public bool? IsClosed { get; set; }

    public virtual ICollection<ComplainDetail> ComplainDetails { get; set; } = new List<ComplainDetail>();

    public virtual ICollection<ComplainMaster> ComplainMasters { get; set; } = new List<ComplainMaster>();
}
