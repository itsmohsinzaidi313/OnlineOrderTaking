using System;
using System.Collections.Generic;

namespace PointofSaleModels.DatabaseModels;

public partial class DiscountBranchMapping
{
    public int DiscountBranchMappingId { get; set; }

    public int DiscountId { get; set; }

    public int BranchId { get; set; }

    public bool IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public string? UserIp { get; set; }

    public virtual BranchMaster Branch { get; set; } = null!;
}
