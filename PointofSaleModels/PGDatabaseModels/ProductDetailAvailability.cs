using System;
using System.Collections.Generic;

namespace PointofSaleModels.PGDatabaseModels;

public partial class ProductDetailAvailability
{
    public int ProductDetailAvailableId { get; set; }

    public int? ProductBranchId { get; set; }

    public int? DayId { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public bool? IsActive { get; set; }

    public virtual SetupMasterDetail? Day { get; set; }

    public virtual ProductDetailBranchMapping? ProductBranch { get; set; }
}
