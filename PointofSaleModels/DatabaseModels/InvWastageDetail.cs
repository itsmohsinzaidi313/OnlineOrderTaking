namespace PointofSaleModels.DatabaseModels;

public partial class InvWastageDetail
{
    public int WastageDetailId { get; set; }

    public int? WastageId { get; set; }

    public int? ProductDetailId { get; set; }

    public double? QtyInLevel1 { get; set; }

    public double? QtyInLevel2 { get; set; }

    public double? TotalQtyInLevel3 { get; set; }

    public int? Level1UnitId { get; set; }

    public int? Level2UnitId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public string? UserIp { get; set; }

    public bool IsActive { get; set; }

    public int? BatchId { get; set; }

    public virtual InvBatch? Batch { get; set; }

    public virtual ICollection<InvInventoryStore> InvInventoryStores { get; set; } = new List<InvInventoryStore>();

    public virtual InvSetupUnit? Level1Unit { get; set; }

    public virtual InvSetupUnit? Level2Unit { get; set; }

    public virtual ProductDetail? ProductDetail { get; set; }

    public virtual InvWastageMaster? Wastage { get; set; }
}
