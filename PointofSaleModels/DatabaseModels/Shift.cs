namespace PointofSaleModels.DatabaseModels;

public partial class Shift
{
    public int ShiftId { get; set; }

    public string? ShiftName { get; set; }

    public string? Prefix { get; set; }

    public bool IsActive { get; set; }

    public int? Createdby { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? CompanyId { get; set; }

    public string? UserIp { get; set; }

    public virtual SetupCompany? Company { get; set; }

    public virtual ICollection<ShiftDetail> ShiftDetails { get; set; } = new List<ShiftDetail>();
}
