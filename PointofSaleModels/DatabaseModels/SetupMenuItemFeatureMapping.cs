namespace PointofSaleModels.DatabaseModels;

public partial class SetupMenuItemFeatureMapping
{
    public int MenuItemFeatureId { get; set; }

    public int MenuId { get; set; }

    public int FeatureId { get; set; }

    public bool IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public string? UserIp { get; set; }

    public virtual SetupFeature Feature { get; set; } = null!;

    public virtual SetupMenuItem Menu { get; set; } = null!;

    public virtual ICollection<SetupRoleMenuItemFeatureMapping> SetupRoleMenuItemFeatureMappings { get; set; } = new List<SetupRoleMenuItemFeatureMapping>();
}
