using System;
using System.Collections.Generic;

namespace PointofSaleModels.DatabaseModels;

public partial class ChartOfAccount
{
    public int ChartOfAccountId { get; set; }

    public string AccountCode { get; set; } = null!;

    public string AccountName { get; set; } = null!;

    public bool IsActive { get; set; }

    public bool IsTransactionLevel { get; set; }

    public DateTime CreatedDate { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public string UserIp { get; set; } = null!;

    public int? ParentId { get; set; }

    public decimal? OpeningBalance { get; set; }

    public int NatureOfAccountId { get; set; }

    public int? CompanyId { get; set; }

    public virtual ICollection<PaymentVoucherDetail> PaymentVoucherDetails { get; set; } = new List<PaymentVoucherDetail>();
}
