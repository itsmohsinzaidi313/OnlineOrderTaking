using System;
using System.Collections.Generic;

namespace PointofSaleModels.PGDatabaseModels;

public partial class Flavour
{
    public int FlavourId { get; set; }

    public string? FlavourName { get; set; }

    public int? CompanyId { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<ProductDetail> ProductDetails { get; set; } = new List<ProductDetail>();
}
