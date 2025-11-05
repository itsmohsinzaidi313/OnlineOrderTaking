namespace PointofSaleModels.DatabaseModels;

public partial class UserRole
{
    public int RoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public int? CompanyId { get; set; }

    public DateTime CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public bool IsActive { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public string UserIp { get; set; } = null!;

    public bool IsDefaultRole { get; set; }

    public bool? IsPos { get; set; }

    public virtual SetupCompany? Company { get; set; }

    public virtual ICollection<SetupRoleAccessAction> SetupRoleAccessActions { get; set; } = new List<SetupRoleAccessAction>();
}
