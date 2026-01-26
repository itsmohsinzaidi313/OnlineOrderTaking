using System;
using System.Collections.Generic;

namespace PointofSaleModels.PGDatabaseModels;

public partial class BranchDetail
{
    public int BranchDetailId { get; set; }

    public int BranchId { get; set; }

    public int AreaId { get; set; }

    public string? AreaName { get; set; }

    public TimeSpan? AreaStartTime { get; set; }

    public TimeSpan? AreaEndTime { get; set; }

    public int? DeliveryTime { get; set; }

    public double? MinimumOrder { get; set; }

    public double? DeliveryCharges { get; set; }

    public bool? IsEnabled { get; set; }

    public double? DeliveryChargesWaiveOffLimit { get; set; }

    public bool IsActive { get; set; }
}
