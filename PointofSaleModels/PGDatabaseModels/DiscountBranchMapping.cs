using System;
using System.Collections.Generic;

namespace PointofSaleModels.PGDatabaseModels;

public partial class DiscountBranchMapping
{
    public int DiscountBranchMappingId { get; set; }

    public int DiscountId { get; set; }

    public int BranchId { get; set; }

    public bool IsActive { get; set; }

    public virtual Discount Discount { get; set; } = null!;
}
