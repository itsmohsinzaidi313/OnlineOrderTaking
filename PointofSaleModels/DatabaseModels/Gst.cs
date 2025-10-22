using System;
using System.Collections.Generic;

namespace PointofSaleModels.DatabaseModels;

public partial class Gst
{
    public int Gstid { get; set; }

    public double? Gstpercentage { get; set; }

    public int? CityId { get; set; }

    public int? CompanyId { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public string? UserIp { get; set; }

    public string? Gstname { get; set; }

    public int? PaymentModeId { get; set; }

    public virtual City? City { get; set; }

    public virtual SetupCompany? Company { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<OrderMaster> OrderMasters { get; set; } = new List<OrderMaster>();

    public virtual PaymentMode? PaymentMode { get; set; }
}
