using System;
using System.Collections.Generic;

namespace PointofSaleModels.DatabaseModels;

public partial class ProductDetailCode
{
    public int ProductDetailCodeId { get; set; }

    public int? ProductDetailId { get; set; }

    public string? ProductCode { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? UserIp { get; set; }

    public virtual ProductDetail? ProductDetail { get; set; }
}
