using System;
using System.Collections.Generic;

namespace PointofSaleModels.DatabaseModels;

public partial class InvBatch
{
    public int BatchId { get; set; }

    public int? ProductDetailId { get; set; }

    public string? BatchNumber { get; set; }

    public double Quantity { get; set; }

    public double Price { get; set; }

    public DateTime? ManufactureDate { get; set; }

    public DateTime? ExpiryDate { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? UserIp { get; set; }

    public int? CompanyId { get; set; }

    public string? Barcode { get; set; }

    public int? GoodReceivingDetailId { get; set; }

    public bool IsDefaultBatch { get; set; }

    public int? ProductionDetailId { get; set; }

    public virtual SetupCompany? Company { get; set; }

    public virtual ICollection<InvAdjustmentDetail> InvAdjustmentDetails { get; set; } = new List<InvAdjustmentDetail>();

    public virtual ICollection<InvClosingDetail> InvClosingDetails { get; set; } = new List<InvClosingDetail>();

    public virtual ICollection<InvConsumptionBatchDetail> InvConsumptionBatchDetails { get; set; } = new List<InvConsumptionBatchDetail>();

    public virtual ICollection<InvGoodReceivingReturnDetail> InvGoodReceivingReturnDetails { get; set; } = new List<InvGoodReceivingReturnDetail>();

    public virtual ICollection<InvInventoryStore> InvInventoryStores { get; set; } = new List<InvInventoryStore>();

    public virtual ICollection<InvIssuenceDetail> InvIssuenceDetails { get; set; } = new List<InvIssuenceDetail>();

    public virtual ICollection<InvPodetail> InvPodetails { get; set; } = new List<InvPodetail>();

    public virtual ICollection<InvReceivingDetail> InvReceivingDetails { get; set; } = new List<InvReceivingDetail>();

    public virtual ICollection<InvSubRecipeProductionDetail> InvSubRecipeProductionDetails { get; set; } = new List<InvSubRecipeProductionDetail>();

    public virtual ICollection<InvTransferDetail> InvTransferDetails { get; set; } = new List<InvTransferDetail>();

    public virtual ICollection<InvTransitDetail> InvTransitDetails { get; set; } = new List<InvTransitDetail>();

    public virtual ICollection<InvWastageDetail> InvWastageDetails { get; set; } = new List<InvWastageDetail>();

    public virtual ProductDetail? ProductDetail { get; set; }

    public virtual InvSubRecipeProductionDetail? ProductionDetail { get; set; }

    public virtual ICollection<SalesReturnDetail> SalesReturnDetails { get; set; } = new List<SalesReturnDetail>();
}
