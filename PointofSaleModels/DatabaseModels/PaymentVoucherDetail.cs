using System;
using System.Collections.Generic;

namespace PointofSaleModels.DatabaseModels;

public partial class PaymentVoucherDetail
{
    public int PaymentVoucherDetailId { get; set; }

    public int? PaymentVoucherMasterId { get; set; }

    public int? ChartOfAccountId { get; set; }

    public int? CostCenterId { get; set; }

    public string? InvoiceNo { get; set; }

    public decimal? Debit { get; set; }

    public decimal? Credit { get; set; }

    public string? Rate { get; set; }

    public string? AccountNo { get; set; }

    public int? PymentTypeId { get; set; }

    public string? ChequeNo { get; set; }

    public bool? IsActive { get; set; }

    public DateTime CreatedDate { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public string UserIp { get; set; } = null!;

    public string? Description { get; set; }

    public int? OrderMasterId { get; set; }

    public virtual ChartOfAccount? ChartOfAccount { get; set; }

    public virtual CostCenter? CostCenter { get; set; }

    public virtual OrderMaster? OrderMaster { get; set; }

    public virtual PaymentVoucherMaster? PaymentVoucherMaster { get; set; }

    public virtual PaymentMode? PymentType { get; set; }
}
