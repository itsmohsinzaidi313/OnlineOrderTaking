namespace PointofSaleModels.DatabaseModels;

public partial class OrderStatusLog
{
    public int OrderStatusLogId { get; set; }

    public int OrderMasterId { get; set; }

    public int OrderStatusId { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedDate { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public string? UserIp { get; set; }

    public bool IsActive { get; set; }

    public int? CompanyId { get; set; }

    public virtual SetupCompany? Company { get; set; }

    public virtual UserLogin CreatedByNavigation { get; set; } = null!;

    public virtual OrderMaster OrderMaster { get; set; } = null!;

    public virtual OrderStatus OrderStatus { get; set; } = null!;
}
