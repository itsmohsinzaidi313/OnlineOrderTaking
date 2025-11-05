namespace PointofSaleModels.DatabaseModels;

public partial class SetupRoleAccessAction
{
    public int Id { get; set; }

    public int? SetupDetailId { get; set; }

    public int? RoleId { get; set; }

    public int? CompanyId { get; set; }

    public bool? IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? UserIp { get; set; }

    public bool IsAccess { get; set; }

    public virtual SetupCompany? Company { get; set; }

    public virtual UserRole? Role { get; set; }

    public virtual SetupMasterDetail? SetupDetail { get; set; }
}
