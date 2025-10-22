using System;
using System.Collections.Generic;

namespace PointofSaleModels.DatabaseModels;

public partial class ProductDetail
{
    public int ProductDetailId { get; set; }

    public int ProductId { get; set; }

    public int SizeId { get; set; }

    public double Price { get; set; }

    public double TaxPercent { get; set; }

    public DateTime CreatedDate { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public string? UserIp { get; set; }

    public bool IsActive { get; set; }

    public bool OnlyForDeal { get; set; }

    public bool IsEnable { get; set; }

    public int? FlavourId { get; set; }

    public bool IsTopping { get; set; }

    public bool IsSaleable { get; set; }

    public bool IsProduction { get; set; }

    public int? PurchaseUnitId { get; set; }

    public int? IssuanceUnitId { get; set; }

    public int? ConsumeUnitId { get; set; }

    public double? PurchaseIssueConversion { get; set; }

    public double? IssueConsumeConversion { get; set; }

    public string? Sku { get; set; }

    public int? ParentProductDetailId { get; set; }

    public double? ReOrderQuantityInConsume { get; set; }

    public bool IsInventoryItem { get; set; }

    public double? FuturePrice { get; set; }

    public double? PreviousPrice { get; set; }

    public bool IsDealDirectPunch { get; set; }

    public bool IsOpen { get; set; }

    public bool IsPromotion { get; set; }

    public string? RemoteId { get; set; }

    public bool IsBestSeller { get; set; }

    public double PriceBeforeDiscount { get; set; }

    public virtual InvSetupUnit? ConsumeUnit { get; set; }

    public virtual ICollection<DealDescription> DealDescriptions { get; set; } = new List<DealDescription>();

    public virtual ICollection<DealItemDetail> DealItemDetails { get; set; } = new List<DealItemDetail>();

    public virtual ICollection<DiscountProductDetailMapping> DiscountProductDetailMappings { get; set; } = new List<DiscountProductDetailMapping>();

    public virtual Flavour? Flavour { get; set; }

    public virtual ICollection<InvAdjustmentDetail> InvAdjustmentDetails { get; set; } = new List<InvAdjustmentDetail>();

    public virtual ICollection<InvBatch> InvBatches { get; set; } = new List<InvBatch>();

    public virtual ICollection<InvClosingDetail> InvClosingDetails { get; set; } = new List<InvClosingDetail>();

    public virtual ICollection<InvConsumptionBatchDetail> InvConsumptionBatchDetails { get; set; } = new List<InvConsumptionBatchDetail>();

    public virtual ICollection<InvConsumptionDetail> InvConsumptionDetails { get; set; } = new List<InvConsumptionDetail>();

    public virtual ICollection<InvDemandDetail> InvDemandDetails { get; set; } = new List<InvDemandDetail>();

    public virtual ICollection<InvEmployeeMealDetail> InvEmployeeMealDetails { get; set; } = new List<InvEmployeeMealDetail>();

    public virtual ICollection<InvGoodReceivingDetail> InvGoodReceivingDetails { get; set; } = new List<InvGoodReceivingDetail>();

    public virtual ICollection<InvGoodReceivingReturnDetail> InvGoodReceivingReturnDetails { get; set; } = new List<InvGoodReceivingReturnDetail>();

    public virtual ICollection<InvInventoryStore> InvInventoryStores { get; set; } = new List<InvInventoryStore>();

    public virtual ICollection<InvIssuenceDetail> InvIssuenceDetails { get; set; } = new List<InvIssuenceDetail>();

    public virtual ICollection<InvPodetail> InvPodetails { get; set; } = new List<InvPodetail>();

    public virtual ICollection<InvPurchaseInvoiceDetail> InvPurchaseInvoiceDetails { get; set; } = new List<InvPurchaseInvoiceDetail>();

    public virtual ICollection<InvReceivingDetail> InvReceivingDetails { get; set; } = new List<InvReceivingDetail>();

    public virtual ICollection<InvRecipeDetail> InvRecipeDetails { get; set; } = new List<InvRecipeDetail>();

    public virtual ICollection<InvRecipeMaster> InvRecipeMasterProductDetails { get; set; } = new List<InvRecipeMaster>();

    public virtual ICollection<InvRecipeMaster> InvRecipeMasterSubRecipeItems { get; set; } = new List<InvRecipeMaster>();

    public virtual ICollection<InvRequisitionDetail> InvRequisitionDetails { get; set; } = new List<InvRequisitionDetail>();

    public virtual ICollection<InvSubRecipeProductionDetail> InvSubRecipeProductionDetails { get; set; } = new List<InvSubRecipeProductionDetail>();

    public virtual ICollection<InvTransferDetail> InvTransferDetails { get; set; } = new List<InvTransferDetail>();

    public virtual ICollection<InvTransitDetail> InvTransitDetails { get; set; } = new List<InvTransitDetail>();

    public virtual ICollection<InvWastageDetail> InvWastageDetails { get; set; } = new List<InvWastageDetail>();

    public virtual InvSetupUnit? IssuanceUnit { get; set; }

    public virtual ICollection<OrderDetailLog> OrderDetailLogs { get; set; } = new List<OrderDetailLog>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual Product Product { get; set; } = null!;

    public virtual ICollection<ProductDetailBranchMapping> ProductDetailBranchMappings { get; set; } = new List<ProductDetailBranchMapping>();

    public virtual ICollection<ProductDetailCode> ProductDetailCodes { get; set; } = new List<ProductDetailCode>();

    public virtual ICollection<ProductDetailOrderSourcePriceMapping> ProductDetailOrderSourcePriceMappings { get; set; } = new List<ProductDetailOrderSourcePriceMapping>();

    public virtual ICollection<ProductDetailProperty> ProductDetailProperties { get; set; } = new List<ProductDetailProperty>();

    public virtual ICollection<ProductDetailToppingMapping> ProductDetailToppingMappingProductDetailToppings { get; set; } = new List<ProductDetailToppingMapping>();

    public virtual ICollection<ProductDetailToppingMapping> ProductDetailToppingMappingProductDetails { get; set; } = new List<ProductDetailToppingMapping>();

    public virtual InvSetupUnit? PurchaseUnit { get; set; }

    public virtual ICollection<ReservationDetail> ReservationDetails { get; set; } = new List<ReservationDetail>();

    public virtual ICollection<SalesReturnDetail> SalesReturnDetails { get; set; } = new List<SalesReturnDetail>();

    public virtual ProductSize Size { get; set; } = null!;

    public virtual ICollection<TempOrderDetail> TempOrderDetails { get; set; } = new List<TempOrderDetail>();
}
