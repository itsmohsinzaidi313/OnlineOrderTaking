namespace PointofSaleModels.DatabaseModels;

public partial class InvPurchaseInvoiceMaster
{
    public int PurchaseInvoiceId { get; set; }

    public string? PurchaseInvoiceNumber { get; set; }

    public int? VendorId { get; set; }

    public int? BranchId { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? UserIp { get; set; }

    public double SubTotal { get; set; }

    public double TaxAmount { get; set; }

    public double Discount { get; set; }

    public double NetAmount { get; set; }

    public int? UserId { get; set; }

    public bool IsSubmitted { get; set; }

    public DateTime? Date { get; set; }

    public string? RefNumber { get; set; }

    public virtual BranchMaster? Branch { get; set; }

    public virtual ICollection<InvPurchaseInvoiceDetail> InvPurchaseInvoiceDetails { get; set; } = new List<InvPurchaseInvoiceDetail>();

    public virtual InvSetupVendor? Vendor { get; set; }
}
