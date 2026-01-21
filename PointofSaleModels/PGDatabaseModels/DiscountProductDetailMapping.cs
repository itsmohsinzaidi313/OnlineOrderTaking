using System;
using System.Collections.Generic;

namespace PointofSaleModels.PGDatabaseModels;

public partial class DiscountProductDetailMapping
{
    public int DiscountProductDetailMappingId { get; set; }

    public int DiscountId { get; set; }

    public int ProductDetailId { get; set; }

    public bool IsActive { get; set; }

    public virtual Discount Discount { get; set; } = null!;

    public virtual ProductDetail ProductDetail { get; set; } = null!;
}
