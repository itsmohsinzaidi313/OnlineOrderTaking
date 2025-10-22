using System;
using System.Collections.Generic;

namespace PointofSaleModels.DatabaseModels;

public partial class TableMergeDetail
{
    public int TableMergeDetailId { get; set; }

    public int TableMergeMasterId { get; set; }

    public int OrderMasterId { get; set; }

    public int TableId { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual OrderMaster OrderMaster { get; set; } = null!;

    public virtual Table Table { get; set; } = null!;

    public virtual TableMerge TableMergeMaster { get; set; } = null!;
}
