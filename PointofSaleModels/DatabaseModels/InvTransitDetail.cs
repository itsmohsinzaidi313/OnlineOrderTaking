namespace PointofSaleModels.DatabaseModels;

public partial class InvTransitDetail
{
    public int TransitDetailId { get; set; }

    public int ProductDetailId { get; set; }

    public double TransitQuantity { get; set; }

    public int? TransitUnit { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? UserIp { get; set; }

    public int? BatchId { get; set; }

    public int? IssuanceDetailId { get; set; }

    public int? TypeId { get; set; }

    public int? TransferDetailId { get; set; }

    public virtual InvBatch? Batch { get; set; }

    public virtual ICollection<InvInventoryStore> InvInventoryStores { get; set; } = new List<InvInventoryStore>();

    public virtual InvIssuenceDetail? IssuanceDetail { get; set; }

    public virtual ProductDetail ProductDetail { get; set; } = null!;

    public virtual InvTransferDetail? TransferDetail { get; set; }

    public virtual InvSetupUnit? TransitUnitNavigation { get; set; }

    public virtual SetupMasterDetail? Type { get; set; }
}
