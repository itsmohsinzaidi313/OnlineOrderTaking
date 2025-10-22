using System;
using System.Collections.Generic;

namespace PointofSaleModels.DatabaseModels;

public partial class LoyaltyCard
{
    public int LoyaltyCardId { get; set; }

    public string LoyaltyCardNumber { get; set; } = null!;

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public int? CustomerId { get; set; }

    public int LoyaltyCardTypeId { get; set; }

    public bool IsActive { get; set; }

    public bool IsEnable { get; set; }

    public DateTime CreatedDate { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public string? UserIp { get; set; }

    public int? CompanyId { get; set; }

    public virtual SetupCompany? Company { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<LoyaltyCardBalance> LoyaltyCardBalances { get; set; } = new List<LoyaltyCardBalance>();

    public virtual LoyaltyCardType LoyaltyCardType { get; set; } = null!;
}
