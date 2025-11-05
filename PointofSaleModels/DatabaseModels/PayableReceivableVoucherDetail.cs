namespace PointofSaleModels.DatabaseModels;

public partial class PayableReceivableVoucherDetail
{
    public int PayableReceivableVoucherDetailId { get; set; }

    public int? PayableReceivableVoucherMasterId { get; set; }

    public string? InvoiceNo { get; set; }

    public decimal? Debit { get; set; }

    public decimal? Credit { get; set; }

    public int? PaymentModeId { get; set; }

    public string? ChequeNo { get; set; }

    public bool? IsActive { get; set; }

    public DateTime CreatedDate { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public string UserIp { get; set; } = null!;

    public int? OrderMasterId { get; set; }

    public int? PurchaseInvoiceId { get; set; }

    public virtual OrderMaster? OrderMaster { get; set; }

    public virtual PayableReceivableVoucherMaster? PayableReceivableVoucherMaster { get; set; }
}
