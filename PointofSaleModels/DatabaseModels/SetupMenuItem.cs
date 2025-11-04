namespace PointofSaleModels.DatabaseModels;

public partial class SetupMenuItem
{
    public int MenuId { get; set; }

    public string MenuName { get; set; } = null!;

    public string? MenuUrl { get; set; }

    public int? ParentId { get; set; }

    public bool? IsDisplayedInMenu { get; set; }

    public int? SortOrder { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public bool IsActive { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public string? UserIp { get; set; }

    public string? IconClass { get; set; }

    public virtual ICollection<SetupMenuItem> InverseParent { get; set; } = new List<SetupMenuItem>();

    public virtual SetupMenuItem? Parent { get; set; }

    public virtual ICollection<SetupMenuItemFeatureMapping> SetupMenuItemFeatureMappings { get; set; } = new List<SetupMenuItemFeatureMapping>();
}
