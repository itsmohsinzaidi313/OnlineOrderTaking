namespace PointofSaleModels.DatabaseModels;

public partial class OrderStatusModeMapping
{
    public int OrderStatusModeMappingId { get; set; }

    public int OrderStatusId { get; set; }

    public int OrderModeId { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedDate { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public string UserIp { get; set; } = null!;

    public virtual SetupMasterDetail OrderMode { get; set; } = null!;

    public virtual OrderStatus OrderStatus { get; set; } = null!;
}
