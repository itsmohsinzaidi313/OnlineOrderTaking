namespace PointofSaleModels.DatabaseModels;

public partial class ComplainCategory
{
    public int ComplainCategoryId { get; set; }

    public string? ComplainCategoryName { get; set; }

    public int? ComplainTypeId { get; set; }

    public DateTime CreatedDate { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public string? UserIp { get; set; }

    public bool IsActive { get; set; }

    public int? CompanyId { get; set; }

    public virtual SetupCompany? Company { get; set; }

    public virtual ICollection<ComplainMaster> ComplainMasters { get; set; } = new List<ComplainMaster>();

    public virtual SetupMasterDetail? ComplainType { get; set; }
}
