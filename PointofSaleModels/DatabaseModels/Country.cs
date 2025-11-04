namespace PointofSaleModels.DatabaseModels;

public partial class Country
{
    public int CountryId { get; set; }

    public string CountryName { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public string? UserIp { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<City> Cities { get; set; } = new List<City>();

    public virtual ICollection<Province> Provinces { get; set; } = new List<Province>();

    public virtual ICollection<SetupCompany> SetupCompanies { get; set; } = new List<SetupCompany>();
}
