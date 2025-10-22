using System;
using System.Collections.Generic;

namespace PointofSaleModels.DatabaseModels;

public partial class LoyaltyCardBalance
{
    public int LoyaltyCardBalanceId { get; set; }

    public int? LoyaltyCardId { get; set; }

    public double? Points { get; set; }

    public double? Amount { get; set; }

    public bool? IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? UserIp { get; set; }

    public int? OrderMasterId { get; set; }

    public int? TypeId { get; set; }

    public int? CompanyId { get; set; }

    public virtual SetupCompany? Company { get; set; }

    public virtual LoyaltyCard? LoyaltyCard { get; set; }

    public virtual OrderMaster? OrderMaster { get; set; }

    public virtual SetupMasterDetail? Type { get; set; }
}
