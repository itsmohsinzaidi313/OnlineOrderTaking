using System;
using System.Collections.Generic;

namespace PointofSaleModels.PGDatabaseModels;

public partial class DealDescription
{
    public int DealDescId { get; set; }

    public int? DealItemId { get; set; }

    public int? ProductDetailId { get; set; }

    public bool? IsActive { get; set; }

    public int? SortOrder { get; set; }

    public double? Price { get; set; }
}
