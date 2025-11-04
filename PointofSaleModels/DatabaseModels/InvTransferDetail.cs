namespace PointofSaleModels.DatabaseModels;

public partial class InvTransferDetail
{
    public int TransferDetailId { get; set; }

    public int TransferId { get; set; }

    public int ProductDetailId { get; set; }

    public double? QtyInLevel1 { get; set; }

    public double? QtyInLevel2 { get; set; }

    public double? TotalQtyInLevel3 { get; set; }

    public int? Level1UnitId { get; set; }

    public int? Level2UnitId { get; set; }

    public int? Level3UnitId { get; set; }

    public DateTime CreatedDate { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public string? UserIp { get; set; }

    public bool IsActive { get; set; }

    public int? BatchId { get; set; }

    public virtual InvBatch? Batch { get; set; }

    public virtual ICollection<InvReceivingDetail> InvReceivingDetails { get; set; } = new List<InvReceivingDetail>();

    public virtual ICollection<InvTransitDetail> InvTransitDetails { get; set; } = new List<InvTransitDetail>();

    public virtual InvSetupUnit? Level1Unit { get; set; }

    public virtual InvSetupUnit? Level2Unit { get; set; }

    public virtual InvSetupUnit? Level3Unit { get; set; }

    public virtual ProductDetail ProductDetail { get; set; } = null!;

    public virtual InvTransferMaster Transfer { get; set; } = null!;
}
