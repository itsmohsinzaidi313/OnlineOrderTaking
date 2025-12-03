namespace PointofSaleModels.PGDatabaseModels;

public partial class City
{
    public int CityId { get; set; }

    public string CityName { get; set; } = null!;

    public int? CountryId { get; set; }

    public int? ProvinceId { get; set; }

    public virtual ICollection<CustomerAddressDetail> CustomerAddressDetails { get; set; } = new List<CustomerAddressDetail>();

    public virtual ICollection<Gst> Gsts { get; set; } = new List<Gst>();
}
