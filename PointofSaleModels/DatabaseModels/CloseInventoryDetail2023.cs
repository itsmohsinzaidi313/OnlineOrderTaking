using System;
using System.Collections.Generic;

namespace PointofSaleModels.DatabaseModels;

public partial class CloseInventoryDetail2023
{
    public int? ProductDetailId { get; set; }

    public int? BatchId { get; set; }

    public double? IssueQuantity { get; set; }

    public int? IssueUnitId { get; set; }
}
