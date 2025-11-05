namespace PointofSaleModels.DatabaseModels;

public partial class SetupCompany
{
    public int CompanyId { get; set; }

    public string CompanyName { get; set; } = null!;

    public string? CompanyLogo { get; set; }

    public bool IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public string? UserIp { get; set; }

    public int? NoOfTerminals { get; set; }

    public int? BusinessTypeId { get; set; }

    public int? CurrencyId { get; set; }

    public string? CompanyCode { get; set; }

    public int CountryId { get; set; }

    public string? EmailAddress { get; set; }

    public string? Contact1 { get; set; }

    public string? Contact2 { get; set; }

    public bool? IsEnable { get; set; }

    public bool IsPos { get; set; }

    public bool IsSrbintegration { get; set; }

    public bool IsFbrIntegration { get; set; }

    public bool IsValidCompany { get; set; }

    public string? Msg { get; set; }

    public virtual ICollection<Area> Areas { get; set; } = new List<Area>();

    public virtual ICollection<BranchMaster> BranchMasters { get; set; } = new List<BranchMaster>();

    public virtual SetupMasterDetail? BusinessType { get; set; }

    public virtual ICollection<CompanyPocDetail> CompanyPocDetails { get; set; } = new List<CompanyPocDetail>();

    public virtual ICollection<ComplainCategory> ComplainCategories { get; set; } = new List<ComplainCategory>();

    public virtual ICollection<ComplainMaster> ComplainMasters { get; set; } = new List<ComplainMaster>();

    public virtual Country Country { get; set; } = null!;

    public virtual SetupMasterDetail? Currency { get; set; }

    public virtual ICollection<CustomerAddressDetail> CustomerAddressDetails { get; set; } = new List<CustomerAddressDetail>();

    public virtual ICollection<CustomerPhone> CustomerPhones { get; set; } = new List<CustomerPhone>();

    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();

    public virtual ICollection<Department> Departments { get; set; } = new List<Department>();

    public virtual ICollection<Discount> Discounts { get; set; } = new List<Discount>();

    public virtual ICollection<Flavour> Flavours { get; set; } = new List<Flavour>();

    public virtual ICollection<Gst> Gsts { get; set; } = new List<Gst>();

    public virtual ICollection<InvBatch> InvBatches { get; set; } = new List<InvBatch>();

    public virtual ICollection<InvEmployeeMealMaster> InvEmployeeMealMasters { get; set; } = new List<InvEmployeeMealMaster>();

    public virtual ICollection<InvRecipeMaster> InvRecipeMasters { get; set; } = new List<InvRecipeMaster>();

    public virtual ICollection<InvSetupUnit> InvSetupUnits { get; set; } = new List<InvSetupUnit>();

    public virtual ICollection<LoyaltyCardBalance> LoyaltyCardBalances { get; set; } = new List<LoyaltyCardBalance>();

    public virtual ICollection<LoyaltyCardType> LoyaltyCardTypes { get; set; } = new List<LoyaltyCardType>();

    public virtual ICollection<LoyaltyCard> LoyaltyCards { get; set; } = new List<LoyaltyCard>();

    public virtual ICollection<OrderMaster> OrderMasters { get; set; } = new List<OrderMaster>();

    public virtual ICollection<OrderModeOrderSourceMapping> OrderModeOrderSourceMappings { get; set; } = new List<OrderModeOrderSourceMapping>();

    public virtual ICollection<OrderStatusLog> OrderStatusLogs { get; set; } = new List<OrderStatusLog>();

    public virtual ICollection<PaymentModeOrderSourceMapping> PaymentModeOrderSourceMappings { get; set; } = new List<PaymentModeOrderSourceMapping>();

    public virtual ICollection<PaymentMode> PaymentModes { get; set; } = new List<PaymentMode>();

    public virtual ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();

    public virtual ICollection<ProductSize> ProductSizes { get; set; } = new List<ProductSize>();

    public virtual ICollection<ReservationMaster> ReservationMasters { get; set; } = new List<ReservationMaster>();

    public virtual ICollection<SalesReturnMaster> SalesReturnMasters { get; set; } = new List<SalesReturnMaster>();

    public virtual ICollection<SetupCompanySetting> SetupCompanySettings { get; set; } = new List<SetupCompanySetting>();

    public virtual ICollection<SetupExtraCharge> SetupExtraCharges { get; set; } = new List<SetupExtraCharge>();

    public virtual ICollection<SetupMasterDetail> SetupMasterDetails { get; set; } = new List<SetupMasterDetail>();

    public virtual ICollection<SetupProductTag> SetupProductTags { get; set; } = new List<SetupProductTag>();

    public virtual ICollection<SetupRoleAccessAction> SetupRoleAccessActions { get; set; } = new List<SetupRoleAccessAction>();

    public virtual ICollection<SetupRoleAccess> SetupRoleAccesses { get; set; } = new List<SetupRoleAccess>();

    public virtual ICollection<Shift> Shifts { get; set; } = new List<Shift>();

    public virtual ICollection<TempOrderMaster> TempOrderMasters { get; set; } = new List<TempOrderMaster>();

    public virtual ICollection<Template> Templates { get; set; } = new List<Template>();

    public virtual ICollection<Terminal> Terminals { get; set; } = new List<Terminal>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
