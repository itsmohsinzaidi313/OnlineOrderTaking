namespace PointofSaleModels.DatabaseModels;

public partial class InvConsumptionDetail
{
    public int ConsumptionDetailId { get; set; }

    public int? ConsumptionId { get; set; }

    public int? ProductDetailId { get; set; }

    public double? TotalQty { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public bool? IsActive { get; set; }

    public string? UserIp { get; set; }

    public double RefundQty { get; set; }

    public virtual InvConsumptionMaster? Consumption { get; set; }

    public virtual ICollection<InvInventoryStore> InvInventoryStores { get; set; } = new List<InvInventoryStore>();

    public virtual ProductDetail? ProductDetail { get; set; }
}
