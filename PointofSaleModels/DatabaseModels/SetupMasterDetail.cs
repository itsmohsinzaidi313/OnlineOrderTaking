namespace PointofSaleModels.DatabaseModels;

public partial class SetupMasterDetail
{
    public int SetupDetailId { get; set; }

    public int? SetupMasterId { get; set; }

    public string? SetupDetailName { get; set; }

    public int? ParentId { get; set; }

    public string? Flex1 { get; set; }

    public string? Flex2 { get; set; }

    public string? Flex3 { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? UserIp { get; set; }

    public int? CompanyId { get; set; }

    public virtual ICollection<BranchOrderSourceMapping> BranchOrderSourceMappings { get; set; } = new List<BranchOrderSourceMapping>();

    public virtual SetupCompany? Company { get; set; }

    public virtual ICollection<ComplainCategory> ComplainCategories { get; set; } = new List<ComplainCategory>();

    public virtual ICollection<ComplainMaster> ComplainMasters { get; set; } = new List<ComplainMaster>();

    public virtual ICollection<CustomerAddressDetail> CustomerAddressDetailAddressTypes { get; set; } = new List<CustomerAddressDetail>();

    public virtual ICollection<CustomerAddressDetail> CustomerAddressDetailCaptions { get; set; } = new List<CustomerAddressDetail>();

    public virtual ICollection<CustomerPhone> CustomerPhones { get; set; } = new List<CustomerPhone>();

    public virtual ICollection<DealItemDetail> DealItemDetails { get; set; } = new List<DealItemDetail>();

    public virtual ICollection<DiscountDayMapping> DiscountDayMappings { get; set; } = new List<DiscountDayMapping>();

    public virtual ICollection<DiscountOrderModeMapping> DiscountOrderModeMappings { get; set; } = new List<DiscountOrderModeMapping>();

    public virtual ICollection<DiscountOrderTypeMapping> DiscountOrderTypeMappings { get; set; } = new List<DiscountOrderTypeMapping>();

    public virtual ICollection<InvAdjustmentDetail> InvAdjustmentDetails { get; set; } = new List<InvAdjustmentDetail>();

    public virtual ICollection<InvGoodReceivingMaster> InvGoodReceivingMasters { get; set; } = new List<InvGoodReceivingMaster>();

    public virtual ICollection<InvGoodReceivingReturnMaster> InvGoodReceivingReturnMasters { get; set; } = new List<InvGoodReceivingReturnMaster>();

    public virtual ICollection<InvInventoryStore> InvInventoryStores { get; set; } = new List<InvInventoryStore>();

    public virtual ICollection<InvPomaster> InvPomasters { get; set; } = new List<InvPomaster>();

    public virtual ICollection<InvRecipeDetail> InvRecipeDetails { get; set; } = new List<InvRecipeDetail>();

    public virtual ICollection<InvRequisitionMaster> InvRequisitionMasters { get; set; } = new List<InvRequisitionMaster>();

    public virtual ICollection<InvTransitDetail> InvTransitDetails { get; set; } = new List<InvTransitDetail>();

    public virtual ICollection<LoyaltyCardBalance> LoyaltyCardBalances { get; set; } = new List<LoyaltyCardBalance>();

    public virtual ICollection<OrderDetail> OrderDetailCommisionTypes { get; set; } = new List<OrderDetail>();

    public virtual ICollection<OrderDetailLog> OrderDetailLogs { get; set; } = new List<OrderDetailLog>();

    public virtual ICollection<OrderDetail> OrderDetailProductProperties { get; set; } = new List<OrderDetail>();

    public virtual ICollection<OrderMaster> OrderMasterFinishWasteReasons { get; set; } = new List<OrderMaster>();

    public virtual ICollection<OrderMaster> OrderMasterOrderCancelReasons { get; set; } = new List<OrderMaster>();

    public virtual ICollection<OrderMaster> OrderMasterOrderModes { get; set; } = new List<OrderMaster>();

    public virtual ICollection<OrderMaster> OrderMasterOrderSources { get; set; } = new List<OrderMaster>();

    public virtual ICollection<OrderModeOrderSourceMapping> OrderModeOrderSourceMappingOrderModes { get; set; } = new List<OrderModeOrderSourceMapping>();

    public virtual ICollection<OrderModeOrderSourceMapping> OrderModeOrderSourceMappingOrderSources { get; set; } = new List<OrderModeOrderSourceMapping>();

    public virtual ICollection<OrderStatusModeMapping> OrderStatusModeMappings { get; set; } = new List<OrderStatusModeMapping>();

    public virtual ICollection<PaymentModeOrderSourceMapping> PaymentModeOrderSourceMappingOrderSources { get; set; } = new List<PaymentModeOrderSourceMapping>();

    public virtual ICollection<PaymentModeOrderSourceMapping> PaymentModeOrderSourceMappingPaymentModes { get; set; } = new List<PaymentModeOrderSourceMapping>();

    public virtual ICollection<PaymentVoucherMaster> PaymentVoucherMasters { get; set; } = new List<PaymentVoucherMaster>();

    public virtual ICollection<PrinterDepartmentMapping> PrinterDepartmentMappings { get; set; } = new List<PrinterDepartmentMapping>();

    public virtual ICollection<ProductDetailAvailability> ProductDetailAvailabilities { get; set; } = new List<ProductDetailAvailability>();

    public virtual ICollection<ProductDetailOrderSourcePriceMapping> ProductDetailOrderSourcePriceMappings { get; set; } = new List<ProductDetailOrderSourcePriceMapping>();

    public virtual ICollection<ProductDetailProperty> ProductDetailProperties { get; set; } = new List<ProductDetailProperty>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual ICollection<ReservationMaster> ReservationMasterGuestTypes { get; set; } = new List<ReservationMaster>();

    public virtual ICollection<ReservationMaster> ReservationMasterSlots { get; set; } = new List<ReservationMaster>();

    public virtual ICollection<SetupCompany> SetupCompanyBusinessTypes { get; set; } = new List<SetupCompany>();

    public virtual ICollection<SetupCompany> SetupCompanyCurrencies { get; set; } = new List<SetupCompany>();

    public virtual ICollection<SetupCompanySetting> SetupCompanySettings { get; set; } = new List<SetupCompanySetting>();

    public virtual ICollection<SetupExtraCharge> SetupExtraCharges { get; set; } = new List<SetupExtraCharge>();

    public virtual SetupMaster? SetupMaster { get; set; }

    public virtual ICollection<SetupRoleAccessAction> SetupRoleAccessActions { get; set; } = new List<SetupRoleAccessAction>();

    public virtual ICollection<Template> Templates { get; set; } = new List<Template>();
}
