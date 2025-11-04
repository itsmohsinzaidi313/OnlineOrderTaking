namespace PointofSaleModels.DatabaseModels;

public partial class InvGoodReceivingReturnMaster
{
    public int GoodReceivingReturnId { get; set; }

    public string? GoodReceivingReturnNumber { get; set; }

    public int? VendorId { get; set; }

    public int? StatusId { get; set; }

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

    public int? GoodReceivingId { get; set; }

    public virtual BranchMaster? Branch { get; set; }

    public virtual InvGoodReceivingMaster? GoodReceiving { get; set; }

    public virtual ICollection<InvGoodReceivingReturnDetail> InvGoodReceivingReturnDetails { get; set; } = new List<InvGoodReceivingReturnDetail>();

    public virtual SetupMasterDetail? Status { get; set; }

    public virtual InvSetupVendor? Vendor { get; set; }
}
