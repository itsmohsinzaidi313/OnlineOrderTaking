namespace PointofSaleModels.PGDatabaseModels;

public partial class Gst
{
    public int Gstid { get; set; }

    public double? Gstpercentage { get; set; }

    public int? CityId { get; set; }

    public int? CompanyId { get; set; }

    public bool? IsActive { get; set; }

    public string? Gstname { get; set; }

    public int? PaymentModeId { get; set; }
}
