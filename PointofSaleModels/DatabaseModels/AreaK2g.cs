namespace PointofSaleModels.DatabaseModels;

public partial class AreaK2g
{
    public int? AreaId { get; set; }

    public string AreaName { get; set; } = null!;

    public int CityId { get; set; }

    public string? DangerZone { get; set; }

    public bool? IsEnable { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public string? UserIp { get; set; }

    public bool IsActive { get; set; }

    public bool? IsPosdata { get; set; }

    public int? OmsareaId { get; set; }

    public bool? IsAreaDetailControlled { get; set; }
}
