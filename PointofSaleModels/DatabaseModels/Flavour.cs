using System;
using System.Collections.Generic;

namespace PointofSaleModels.DatabaseModels;

public partial class Flavour
{
    public int FlavourId { get; set; }

    public string FlavourName { get; set; } = null!;

    public int? CompanyId { get; set; }

    public DateTime CreatedDate { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public string? UserIp { get; set; }

    public bool IsActive { get; set; }

    public virtual SetupCompany? Company { get; set; }

    public virtual ICollection<ProductDetail> ProductDetails { get; set; } = new List<ProductDetail>();
}
