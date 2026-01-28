namespace PointofSaleModels.PGDatabaseModels;

public partial class BranchMaster
{
    public int BranchId { get; set; }

    public string BranchName { get; set; } = null!;

    public int CompanyId { get; set; }

    public int? CityId { get; set; }

    public string? CityName { get; set; }

    public bool? IsEnable { get; set; }

    public string? Ntnname { get; set; }

    public string? Ntnnumber { get; set; }

    public TimeSpan? BusinessDayStartTime { get; set; }

    public TimeSpan? BusinessDayEndTime { get; set; }

    public bool IsCallCenter { get; set; }

    public string? BranchAddress { get; set; }

    public string? BranchPhoneNumber { get; set; }

    public bool IsActive { get; set; }
}
