using System;
using System.Collections.Generic;

namespace PointofSaleModels.DatabaseModels;

public partial class ProductDetailProperty
{
    public int ProductDetailPropertyId { get; set; }

    public int? ProductDetailId { get; set; }

    public int? ProductPropertyId { get; set; }

    public double? Price { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? UserIp { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ProductDetail? ProductDetail { get; set; }

    public virtual SetupMasterDetail? ProductProperty { get; set; }
}
