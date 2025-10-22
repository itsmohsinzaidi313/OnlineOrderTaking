using System;
using System.Collections.Generic;

namespace PointofSaleModels.DatabaseModels;

public partial class SetupRoleMenuItemFeatureMapping
{
    public int RoleMenuItemFeatureMappingId { get; set; }

    public int? RoleAccessCode { get; set; }

    public int MenuItemFeatureId { get; set; }

    public bool IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public string? UserIp { get; set; }

    public int? RoleId { get; set; }

    public virtual UserLogin? CreatedByNavigation { get; set; }

    public virtual SetupMenuItemFeatureMapping MenuItemFeature { get; set; } = null!;

    public virtual UserLogin? ModifiedByNavigation { get; set; }

    public virtual SetupRoleAccess? RoleAccessCodeNavigation { get; set; }
}
