using System;
using System.Collections.Generic;

namespace PointofSaleModels.DatabaseModels;

public partial class OrderPayment
{
    public int OrderPaymentId { get; set; }

    public int? TerminalDetailId { get; set; }

    public int? OrderMasterId { get; set; }

    public int? PaymentModeId { get; set; }

    public double? TotalAmount { get; set; }

    public double? ReceivedAmount { get; set; }

    public double? ReturnAmount { get; set; }

    public bool? IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public double? Tip { get; set; }

    public virtual OrderMaster? OrderMaster { get; set; }

    public virtual PaymentMode? PaymentMode { get; set; }

    public virtual TerminalDetail? TerminalDetail { get; set; }
}
