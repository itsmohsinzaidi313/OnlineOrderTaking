namespace PointofSaleModels.DatabaseModels;

public partial class Province
{
    public int ProvinceId { get; set; }

    public string? ProvinceName { get; set; }

    public int CountryId { get; set; }

    public DateTime CreatedDate { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public string? UserIp { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<City> Cities { get; set; } = new List<City>();

    public virtual Country Country { get; set; } = null!;
}
