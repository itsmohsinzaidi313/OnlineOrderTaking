using System;
using System.Collections.Generic;

namespace PointofSaleModels.DatabaseModels;

public partial class InvInventoryStore20230113
{
    public int InventoryId { get; set; }

    public int ProductDetailId { get; set; }

    public int BranchId { get; set; }

    public int TypeId { get; set; }

    public double QuantityInConsume { get; set; }

    public bool IsActive { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? UserIp { get; set; }

    public int? GoodReceivingDetailId { get; set; }

    public int? ConsumeUnitId { get; set; }

    public int? BatchId { get; set; }

    public double? TotalStockQuantityInConsume { get; set; }

    public DateTime? InventoryDate { get; set; }

    public int? ClosingDetailId { get; set; }

    public int? IssuanceDetailId { get; set; }

    public int? TransferDetailId { get; set; }

    public int? ReceivingDetailId { get; set; }

    public int? ConsumptionDetailId { get; set; }

    public double? BatchStockQuantity { get; set; }

    public int? AdjustmentDetailId { get; set; }

    public int? TransitDetailId { get; set; }

    public int? WastageDetailId { get; set; }

    public int? ConsumptionBatchDetailId { get; set; }

    public int? SalesReturnDetailId { get; set; }

    public int? ProductionDetailId { get; set; }
}
