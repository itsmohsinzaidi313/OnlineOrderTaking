using System;
using System.Collections.Generic;

namespace PointofSaleModels.DatabaseModels;

public partial class TableMerge
{
    public int TableMergeId { get; set; }

    public int TableId { get; set; }

    public bool AllowUnmerge { get; set; }

    public int BranchId { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int OrderMasterId { get; set; }

    public virtual BranchMaster Branch { get; set; } = null!;

    public virtual OrderMaster OrderMaster { get; set; } = null!;

    public virtual Table Table { get; set; } = null!;

    public virtual ICollection<TableMergeDetail> TableMergeDetails { get; set; } = new List<TableMergeDetail>();
}
