namespace PointofSaleModels.DatabaseModels;

public partial class InvConsumptionBatchDetail20230113
{
    public int ConsumptionBatchDetailId { get; set; }

    public int? ConsumptionId { get; set; }

    public int? ProductDetailId { get; set; }

    public double? TotalQty { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public bool? IsActive { get; set; }

    public string? UserIp { get; set; }

    public int? BatchId { get; set; }

    public double RefundQty { get; set; }

    public double ConsumeUnitPrice { get; set; }

    public double TotalConsumptionAmount { get; set; }
}
