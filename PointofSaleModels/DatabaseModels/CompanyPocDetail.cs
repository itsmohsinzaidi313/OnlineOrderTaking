namespace PointofSaleModels.DatabaseModels;

public partial class CompanyPocDetail
{
    public int PocId { get; set; }

    public string? PocName { get; set; }

    public string? PocContact1 { get; set; }

    public string? PocContact2 { get; set; }

    public string? PocEmailAddress { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? UserIp { get; set; }

    public int? CompanyId { get; set; }

    public virtual SetupCompany? Company { get; set; }
}
