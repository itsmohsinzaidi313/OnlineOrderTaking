using System;
using System.Collections.Generic;

namespace PointofSaleModels.PGDatabaseModels;

public partial class BranchDayMapping
{
    public int BranchDayMappingId { get; set; }

    public int BranchId { get; set; }

    public int DayId { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public bool IsActive { get; set; }

    public virtual SetupMasterDetail Day { get; set; } = null!;
}
