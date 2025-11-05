using System;
using System.Collections.Generic;

namespace PointofSaleModels.PGDatabaseModels;

public partial class Area
{
    public int AreaId { get; set; }

    public string AreaName { get; set; } = null!;

    public int? CityId { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public int CompanyId { get; set; }

    public bool? IsEnable { get; set; }

    public bool? IsActive { get; set; }
}
