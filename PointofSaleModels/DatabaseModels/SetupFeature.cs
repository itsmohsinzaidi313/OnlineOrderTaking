namespace PointofSaleModels.DatabaseModels;

public partial class SetupFeature
{
    public int FeatureId { get; set; }

    public string Feature { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public string? UserIp { get; set; }

    public int? SortOrder { get; set; }

    public virtual ICollection<SetupMenuItemFeatureMapping> SetupMenuItemFeatureMappings { get; set; } = new List<SetupMenuItemFeatureMapping>();
}
