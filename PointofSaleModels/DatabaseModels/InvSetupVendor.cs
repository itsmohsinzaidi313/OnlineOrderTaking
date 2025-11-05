namespace PointofSaleModels.DatabaseModels;

public partial class InvSetupVendor
{
    public int VendorId { get; set; }

    public string? VendorName { get; set; }

    public string? Address { get; set; }

    public string? ContactNumber { get; set; }

    public string? Email { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? UserIp { get; set; }

    public int? UserId { get; set; }

    public bool IsActive { get; set; }

    public int? CompanyId { get; set; }

    public string? Ntn { get; set; }

    public string? Gst { get; set; }

    public string? Sst { get; set; }

    public string? ContactNo { get; set; }

    public bool? IsTaxPayer { get; set; }

    public bool? IsApprovedVendor { get; set; }

    public virtual ICollection<InvGoodReceivingMaster> InvGoodReceivingMasters { get; set; } = new List<InvGoodReceivingMaster>();

    public virtual ICollection<InvGoodReceivingReturnMaster> InvGoodReceivingReturnMasters { get; set; } = new List<InvGoodReceivingReturnMaster>();

    public virtual ICollection<InvPomaster> InvPomasters { get; set; } = new List<InvPomaster>();

    public virtual ICollection<InvPurchaseInvoiceMaster> InvPurchaseInvoiceMasters { get; set; } = new List<InvPurchaseInvoiceMaster>();

    public virtual ICollection<InvSetupVendorPoc> InvSetupVendorPocs { get; set; } = new List<InvSetupVendorPoc>();

    public virtual ICollection<PayableReceivableVoucherMaster> PayableReceivableVoucherMasters { get; set; } = new List<PayableReceivableVoucherMaster>();
}
