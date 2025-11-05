using System;
using System.Collections.Generic;

namespace PointofSaleModels.PGDatabaseModels;

public partial class ProductDetailBranchMapping
{
    public int ProductDetailBranchMappingId { get; set; }

    public int? ProductDetailId { get; set; }

    public int? BranchId { get; set; }

    public bool? IsActive { get; set; }

    public bool IsDayWise { get; set; }

    public bool IsEnable { get; set; }

    public string? RemoteId { get; set; }

    public virtual BranchMaster? Branch { get; set; }

    public virtual ProductDetail? ProductDetail { get; set; }

    public virtual ICollection<ProductDetailAvailability> ProductDetailAvailabilities { get; set; } = new List<ProductDetailAvailability>();
}
