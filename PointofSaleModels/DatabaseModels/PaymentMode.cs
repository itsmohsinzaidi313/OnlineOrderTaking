using System;
using System.Collections.Generic;

namespace PointofSaleModels.DatabaseModels;

public partial class PaymentMode
{
    public int PaymentModeId { get; set; }

    public string? PaymentMode1 { get; set; }

    public int? CompanyId { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? UserIp { get; set; }

    public bool IsFoc { get; set; }

    public bool? IsPosType { get; set; }

    public bool? IsCashType { get; set; }

    public bool IsThirdParty { get; set; }

    public bool InstantDiscount { get; set; }

    public bool IsCreditType { get; set; }

    public bool IsPartyAccount { get; set; }

    public virtual SetupCompany? Company { get; set; }

    public virtual ICollection<Gst> Gsts { get; set; } = new List<Gst>();

    public virtual ICollection<OrderPayment> OrderPayments { get; set; } = new List<OrderPayment>();

    public virtual ICollection<PaymentVoucherDetail> PaymentVoucherDetails { get; set; } = new List<PaymentVoucherDetail>();

    public virtual ICollection<ReservationMaster> ReservationMasters { get; set; } = new List<ReservationMaster>();
}
