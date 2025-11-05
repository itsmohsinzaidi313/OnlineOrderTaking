namespace PointofSaleModels.DatabaseModels;

public partial class InvIssuenceDetail
{
    public int IssuanceDetailId { get; set; }

    public int IssuanceMasterId { get; set; }

    public int ProductDetailId { get; set; }

    public double IssuanceQuantity { get; set; }

    public int? IssuanceUnit { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? UserIp { get; set; }

    public int? DemandDetailId { get; set; }

    public int? BatchId { get; set; }

    public virtual InvBatch? Batch { get; set; }

    public virtual InvDemandDetail? DemandDetail { get; set; }

    public virtual ICollection<InvInventoryStore> InvInventoryStores { get; set; } = new List<InvInventoryStore>();

    public virtual ICollection<InvReceivingDetail> InvReceivingDetails { get; set; } = new List<InvReceivingDetail>();

    public virtual ICollection<InvTransitDetail> InvTransitDetails { get; set; } = new List<InvTransitDetail>();

    public virtual InvIssuanceMaster IssuanceMaster { get; set; } = null!;

    public virtual InvSetupUnit? IssuanceUnitNavigation { get; set; }

    public virtual ProductDetail ProductDetail { get; set; } = null!;
}
