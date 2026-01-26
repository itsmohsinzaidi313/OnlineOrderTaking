using System;
using System.Collections.Generic;

namespace PointofSaleModels.PGDatabaseModels;

public partial class DiscountOrderTypeMapping
{
    public int DiscountOrderTypeMappingId { get; set; }

    public int DiscountId { get; set; }

    public int OrderTypeId { get; set; }

    public bool IsActive { get; set; }
}
