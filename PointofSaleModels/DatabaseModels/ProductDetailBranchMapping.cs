using System;
using System.Collections.Generic;

namespace PointofSaleModels.DatabaseModels;

public partial class ProductDetailBranchMapping
{
    public int ProductDetailBranchMappingId { get; set; }

    public int? ProductDetailId { get; set; }

    public int? BranchId { get; set; }

    public bool? IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? UserIp { get; set; }

    public DateTime? ValidFrom { get; set; }

    public DateTime? ValidTo { get; set; }

    public bool IsDayWise { get; set; }

    public bool IsEnable { get; set; }

    public virtual BranchMaster? Branch { get; set; }

    public virtual ProductDetail? ProductDetail { get; set; }

    public virtual ICollection<ProductDetailAvailability> ProductDetailAvailabilities { get; set; } = new List<ProductDetailAvailability>();
}
