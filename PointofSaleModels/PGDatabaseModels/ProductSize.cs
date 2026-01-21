using System;
using System.Collections.Generic;

namespace PointofSaleModels.PGDatabaseModels;

public partial class ProductSize
{
    public int SizeId { get; set; }

    public string SizeName { get; set; } = null!;

    public int? CompanyId { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<ProductDetail> ProductDetails { get; set; } = new List<ProductDetail>();
}
