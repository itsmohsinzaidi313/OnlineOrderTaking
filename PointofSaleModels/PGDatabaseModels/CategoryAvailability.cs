using System;
using System.Collections.Generic;

namespace PointofSaleModels.PGDatabaseModels;

public partial class CategoryAvailability
{
    public int CategoryAvailableId { get; set; }

    public int? CategoryId { get; set; }

    public int? DayId { get; set; }

    public TimeSpan? StartTime { get; set; }

    public TimeSpan? EndTime { get; set; }

    public bool? IsActive { get; set; }
}
