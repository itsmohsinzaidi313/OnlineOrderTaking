namespace PointofSaleModels.DatabaseModels;

public partial class ComplainDetail
{
    public int ComplainDetailId { get; set; }

    public int ComplainMasterId { get; set; }

    public int? ComplainStatusId { get; set; }

    public string? Remarks { get; set; }

    public DateTime CreatedDate { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public string? UserIp { get; set; }

    public bool IsActive { get; set; }

    public virtual ComplainMaster ComplainMaster { get; set; } = null!;

    public virtual ComplainStatus? ComplainStatus { get; set; }

    public virtual UserLogin CreatedByNavigation { get; set; } = null!;
}
