namespace PointofSaleModels.DatabaseModels;

public partial class SetupRoleAccess
{
    public int RoleAccessCode { get; set; }

    public int RoleId { get; set; }

    public int MenuId { get; set; }

    public bool? HasAccess { get; set; }

    public bool IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public string? UserIp { get; set; }

    public int? CompanyId { get; set; }

    public virtual SetupCompany? Company { get; set; }

    public virtual ICollection<SetupRoleMenuItemFeatureMapping> SetupRoleMenuItemFeatureMappings { get; set; } = new List<SetupRoleMenuItemFeatureMapping>();
}
