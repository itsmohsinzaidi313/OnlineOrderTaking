using System;
using System.Collections.Generic;

namespace PointofSaleModels.DatabaseModels;

public partial class OrderExtraCharge
{
    public int OrderExtraChargesId { get; set; }

    public int OrderMasterId { get; set; }

    public int? ExtraChargesId { get; set; }

    public double? TotalAmount { get; set; }

    public double? Percentage { get; set; }

    public bool? IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? UserIp { get; set; }

    public virtual SetupExtraCharge? ExtraCharges { get; set; }

    public virtual OrderMaster OrderMaster { get; set; } = null!;
}
