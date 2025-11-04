namespace PointofSaleModels.DatabaseModels;

public partial class InvSetupVendorPoc
{
    public int PocId { get; set; }

    public string? PocName { get; set; }

    public string? PocContact { get; set; }

    public string? Email { get; set; }

    public int? VendorId { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public bool IsActive { get; set; }

    public int? CompanyId { get; set; }

    public virtual InvSetupVendor? Vendor { get; set; }
}
