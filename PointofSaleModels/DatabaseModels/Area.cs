namespace PointofSaleModels.DatabaseModels;

public partial class Area
{
    public int AreaId { get; set; }

    public string AreaName { get; set; } = null!;

    public int CityId { get; set; }

    public bool IsEnable { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public string? UserIp { get; set; }

    public bool IsActive { get; set; }

    public int? CompanyId { get; set; }

    public virtual ICollection<BranchDetail> BranchDetails { get; set; } = new List<BranchDetail>();

    public virtual City City { get; set; } = null!;

    public virtual SetupCompany? Company { get; set; }

    public virtual ICollection<CustomerAddressDetail> CustomerAddressDetails { get; set; } = new List<CustomerAddressDetail>();

    public virtual ICollection<DiscountAreaMapping> DiscountAreaMappings { get; set; } = new List<DiscountAreaMapping>();

    public virtual ICollection<OrderMaster> OrderMasters { get; set; } = new List<OrderMaster>();
}
