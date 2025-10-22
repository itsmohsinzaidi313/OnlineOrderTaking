using System;
using System.Collections.Generic;

namespace PointofSaleModels.DatabaseModels;

public partial class ReservationDetail
{
    public int ReservationDetailId { get; set; }

    public int? ReservationId { get; set; }

    public int? ProductDetailId { get; set; }

    public double? Quantity { get; set; }

    public double? Rate { get; set; }

    public double? Amount { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? UserIp { get; set; }

    public virtual ProductDetail? ProductDetail { get; set; }

    public virtual ReservationMaster? Reservation { get; set; }
}
