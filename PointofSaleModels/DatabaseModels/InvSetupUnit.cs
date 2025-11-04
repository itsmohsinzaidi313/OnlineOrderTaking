namespace PointofSaleModels.DatabaseModels;

public partial class InvSetupUnit
{
    public int UnitId { get; set; }

    public string UnitName { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public string? UserIp { get; set; }

    public bool IsActive { get; set; }

    public int? CompanyId { get; set; }

    public virtual SetupCompany? Company { get; set; }

    public virtual ICollection<InvAdjustmentDetail> InvAdjustmentDetailLevel1Units { get; set; } = new List<InvAdjustmentDetail>();

    public virtual ICollection<InvAdjustmentDetail> InvAdjustmentDetailLevel2Units { get; set; } = new List<InvAdjustmentDetail>();

    public virtual ICollection<InvAdjustmentDetail> InvAdjustmentDetailLevel3Units { get; set; } = new List<InvAdjustmentDetail>();

    public virtual ICollection<InvClosingDetail> InvClosingDetailConsumeUnits { get; set; } = new List<InvClosingDetail>();

    public virtual ICollection<InvClosingDetail> InvClosingDetailIssueUnits { get; set; } = new List<InvClosingDetail>();

    public virtual ICollection<InvClosingDetail> InvClosingDetailPurchaseUnits { get; set; } = new List<InvClosingDetail>();

    public virtual ICollection<InvDemandDetail> InvDemandDetailDemandUnitIdInConsumeNavigations { get; set; } = new List<InvDemandDetail>();

    public virtual ICollection<InvDemandDetail> InvDemandDetailDemandUnitIdInIssueNavigations { get; set; } = new List<InvDemandDetail>();

    public virtual ICollection<InvEmployeeMealDetail> InvEmployeeMealDetails { get; set; } = new List<InvEmployeeMealDetail>();

    public virtual ICollection<InvGoodReceivingDetail> InvGoodReceivingDetailConsumeUnits { get; set; } = new List<InvGoodReceivingDetail>();

    public virtual ICollection<InvGoodReceivingDetail> InvGoodReceivingDetailIssueUnits { get; set; } = new List<InvGoodReceivingDetail>();

    public virtual ICollection<InvGoodReceivingDetail> InvGoodReceivingDetailPurchaseUnits { get; set; } = new List<InvGoodReceivingDetail>();

    public virtual ICollection<InvGoodReceivingReturnDetail> InvGoodReceivingReturnDetailConsumeUnits { get; set; } = new List<InvGoodReceivingReturnDetail>();

    public virtual ICollection<InvGoodReceivingReturnDetail> InvGoodReceivingReturnDetailIssueUnits { get; set; } = new List<InvGoodReceivingReturnDetail>();

    public virtual ICollection<InvGoodReceivingReturnDetail> InvGoodReceivingReturnDetailPurchaseUnits { get; set; } = new List<InvGoodReceivingReturnDetail>();

    public virtual ICollection<InvInventoryStore> InvInventoryStores { get; set; } = new List<InvInventoryStore>();

    public virtual ICollection<InvIssuenceDetail> InvIssuenceDetails { get; set; } = new List<InvIssuenceDetail>();

    public virtual ICollection<InvPodetail> InvPodetailConsumeUnits { get; set; } = new List<InvPodetail>();

    public virtual ICollection<InvPodetail> InvPodetailIssueUnits { get; set; } = new List<InvPodetail>();

    public virtual ICollection<InvPodetail> InvPodetailPurchaseUnits { get; set; } = new List<InvPodetail>();

    public virtual ICollection<InvPurchaseInvoiceDetail> InvPurchaseInvoiceDetailConsumeUnits { get; set; } = new List<InvPurchaseInvoiceDetail>();

    public virtual ICollection<InvPurchaseInvoiceDetail> InvPurchaseInvoiceDetailIssueUnits { get; set; } = new List<InvPurchaseInvoiceDetail>();

    public virtual ICollection<InvPurchaseInvoiceDetail> InvPurchaseInvoiceDetailPurchaseUnits { get; set; } = new List<InvPurchaseInvoiceDetail>();

    public virtual ICollection<InvReceivingDetail> InvReceivingDetailLevel1Units { get; set; } = new List<InvReceivingDetail>();

    public virtual ICollection<InvReceivingDetail> InvReceivingDetailLevel2Units { get; set; } = new List<InvReceivingDetail>();

    public virtual ICollection<InvReceivingDetail> InvReceivingDetailLevel3Units { get; set; } = new List<InvReceivingDetail>();

    public virtual ICollection<InvRecipeDetail> InvRecipeDetails { get; set; } = new List<InvRecipeDetail>();

    public virtual ICollection<InvRequisitionDetail> InvRequisitionDetailConsumeUnits { get; set; } = new List<InvRequisitionDetail>();

    public virtual ICollection<InvRequisitionDetail> InvRequisitionDetailIssueUnits { get; set; } = new List<InvRequisitionDetail>();

    public virtual ICollection<InvRequisitionDetail> InvRequisitionDetailPurchaseUnits { get; set; } = new List<InvRequisitionDetail>();

    public virtual ICollection<InvSubRecipeProductionDetail> InvSubRecipeProductionDetailLevel1Units { get; set; } = new List<InvSubRecipeProductionDetail>();

    public virtual ICollection<InvSubRecipeProductionDetail> InvSubRecipeProductionDetailLevel2Units { get; set; } = new List<InvSubRecipeProductionDetail>();

    public virtual ICollection<InvSubRecipeProductionDetail> InvSubRecipeProductionDetailLevel3Units { get; set; } = new List<InvSubRecipeProductionDetail>();

    public virtual ICollection<InvTransferDetail> InvTransferDetailLevel1Units { get; set; } = new List<InvTransferDetail>();

    public virtual ICollection<InvTransferDetail> InvTransferDetailLevel2Units { get; set; } = new List<InvTransferDetail>();

    public virtual ICollection<InvTransferDetail> InvTransferDetailLevel3Units { get; set; } = new List<InvTransferDetail>();

    public virtual ICollection<InvTransitDetail> InvTransitDetails { get; set; } = new List<InvTransitDetail>();

    public virtual ICollection<InvWastageDetail> InvWastageDetailLevel1Units { get; set; } = new List<InvWastageDetail>();

    public virtual ICollection<InvWastageDetail> InvWastageDetailLevel2Units { get; set; } = new List<InvWastageDetail>();

    public virtual ICollection<ProductDetail> ProductDetailConsumeUnits { get; set; } = new List<ProductDetail>();

    public virtual ICollection<ProductDetail> ProductDetailIssuanceUnits { get; set; } = new List<ProductDetail>();

    public virtual ICollection<ProductDetail> ProductDetailPurchaseUnits { get; set; } = new List<ProductDetail>();
}
