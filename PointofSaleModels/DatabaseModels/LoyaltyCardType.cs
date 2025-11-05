namespace PointofSaleModels.DatabaseModels;

public partial class LoyaltyCardType
{
    public int LoyaltyCardTypeId { get; set; }

    public string LoyaltyCardType1 { get; set; } = null!;

    public int CompanyId { get; set; }

    public double AmountEarnByPerPoint { get; set; }

    public double AmountRedeemByPerPoint { get; set; }

    public bool IsActive { get; set; }

    public bool IsEnable { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? UserIp { get; set; }

    public virtual SetupCompany Company { get; set; } = null!;

    public virtual ICollection<LoyaltyCard> LoyaltyCards { get; set; } = new List<LoyaltyCard>();
}
