using System;
using System.Collections.Generic;

namespace PointofSaleModels.DatabaseModels;

public partial class SetupProductTag
{
    public int ProductTagId { get; set; }

    public string? ProductTag { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? UserIp { get; set; }

    public int? CompanyId { get; set; }

    public virtual SetupCompany? Company { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
