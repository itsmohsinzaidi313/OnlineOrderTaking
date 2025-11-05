namespace PointofSaleModels.DatabaseModels;

public partial class PaymentVoucherMaster
{
    public int PaymentVoucherMasterId { get; set; }

    public int VoucherTypeId { get; set; }

    public string VoucherNumber { get; set; } = null!;

    public DateTime VoucherDate { get; set; }

    public int? VendorId { get; set; }

    public string? Description { get; set; }

    public bool IsPostVoucher { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedDate { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public string UserIp { get; set; } = null!;

    public int? VoucherMasterId { get; set; }

    public int MonthId { get; set; }

    public int? CostCenterId { get; set; }

    public int? CustomerId { get; set; }

    public int CompanyId { get; set; }

    public int BranchId { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<PaymentVoucherMaster> InverseVoucherMaster { get; set; } = new List<PaymentVoucherMaster>();

    public virtual ICollection<PaymentVoucherDetail> PaymentVoucherDetails { get; set; } = new List<PaymentVoucherDetail>();

    public virtual PaymentVoucherMaster? VoucherMaster { get; set; }

    public virtual SetupMasterDetail VoucherType { get; set; } = null!;
}
