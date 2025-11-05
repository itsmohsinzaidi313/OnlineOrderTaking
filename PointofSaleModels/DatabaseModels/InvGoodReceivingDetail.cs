namespace PointofSaleModels.DatabaseModels;

public partial class InvGoodReceivingDetail
{
    public int GoodReceivingDetailId { get; set; }

    public int? GoodReceivingId { get; set; }

    public int? ProductDetailId { get; set; }

    public int? PurchaseOrderDetailId { get; set; }

    public double PurchaseUnitPrice { get; set; }

    public double SubTotal { get; set; }

    public double TaxAmount { get; set; }

    public double Discount { get; set; }

    public double NetAmount { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? UserIp { get; set; }

    public int? BatchId { get; set; }

    public double? PurchaseQuantity { get; set; }

    public double? IssueQuantity { get; set; }

    public double? ConsumeQuantity { get; set; }

    public int? PurchaseUnitId { get; set; }

    public int? IssueUnitId { get; set; }

    public int? ConsumeUnitId { get; set; }

    public DateTime? ManufactureDate { get; set; }

    public DateTime? ExpiryDate { get; set; }

    public virtual InvSetupUnit? ConsumeUnit { get; set; }

    public virtual InvGoodReceivingMaster? GoodReceiving { get; set; }

    public virtual ICollection<InvGoodReceivingReturnDetail> InvGoodReceivingReturnDetails { get; set; } = new List<InvGoodReceivingReturnDetail>();

    public virtual ICollection<InvInventoryStore> InvInventoryStores { get; set; } = new List<InvInventoryStore>();

    public virtual ICollection<InvPurchaseInvoiceDetail> InvPurchaseInvoiceDetails { get; set; } = new List<InvPurchaseInvoiceDetail>();

    public virtual InvSetupUnit? IssueUnit { get; set; }

    public virtual ProductDetail? ProductDetail { get; set; }

    public virtual InvSetupUnit? PurchaseUnit { get; set; }
}
