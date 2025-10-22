using System;
using System.Collections.Generic;

namespace PointofSaleModels.DatabaseModels;

public partial class SetupExtraCharge
{
    public int ExtraChargesId { get; set; }

    public string? ExtraChargesName { get; set; }

    public int? OrderModeId { get; set; }

    public bool IsPercent { get; set; }

    public double? ChargesValue { get; set; }

    public int CompanyId { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? UserIp { get; set; }

    public virtual SetupCompany Company { get; set; } = null!;

    public virtual ICollection<OrderExtraCharge> OrderExtraCharges { get; set; } = new List<OrderExtraCharge>();

    public virtual SetupMasterDetail? OrderMode { get; set; }
}
