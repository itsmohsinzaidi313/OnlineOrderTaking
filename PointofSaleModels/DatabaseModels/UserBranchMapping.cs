using System;
using System.Collections.Generic;

namespace PointofSaleModels.DatabaseModels;

public partial class UserBranchMapping
{
    public int UserBranchId { get; set; }

    public int? UserId { get; set; }

    public int? BranchId { get; set; }

    public bool? IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? UserIp { get; set; }

    public virtual BranchMaster? Branch { get; set; }

    public virtual UserLogin? User { get; set; }
}
