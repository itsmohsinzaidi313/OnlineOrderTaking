namespace PointofSaleModels.PGDatabaseModels;

public partial class Area
{
    public int AreaId { get; set; }

    public string AreaName { get; set; } = null!;

    public int? CityId { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public int CompanyId { get; set; }

    public bool? IsEnable { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<CustomerAddressDetail> CustomerAddressDetails { get; set; } = new List<CustomerAddressDetail>();

    public virtual ICollection<OrderMaster> OrderMasters { get; set; } = new List<OrderMaster>();
}
