using Microsoft.EntityFrameworkCore;

namespace PointofSaleModels.DatabaseModels;

public partial class RestaurantErpWebContext : DbContext
{
    public RestaurantErpWebContext()
    {
    }

    public RestaurantErpWebContext(DbContextOptions<RestaurantErpWebContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Area> Areas { get; set; }

    public virtual DbSet<AreaK2g> AreaK2gs { get; set; }

    public virtual DbSet<BranchDetail> BranchDetails { get; set; }

    public virtual DbSet<BranchDetailK2g> BranchDetailK2gs { get; set; }

    public virtual DbSet<BranchMaster> BranchMasters { get; set; }

    public virtual DbSet<BranchOrderSourceMapping> BranchOrderSourceMappings { get; set; }

    public virtual DbSet<BulkRetail20221014> BulkRetail20221014s { get; set; }

    public virtual DbSet<BusinessDay> BusinessDays { get; set; }

    public virtual DbSet<ChartOfAccount> ChartOfAccounts { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<CloseInventoryDetail2023> CloseInventoryDetail2023s { get; set; }

    public virtual DbSet<CompanyPocDetail> CompanyPocDetails { get; set; }

    public virtual DbSet<ComplainCategory> ComplainCategories { get; set; }

    public virtual DbSet<ComplainDetail> ComplainDetails { get; set; }

    public virtual DbSet<ComplainMaster> ComplainMasters { get; set; }

    public virtual DbSet<ComplainStatus> ComplainStatuses { get; set; }

    public virtual DbSet<CostCenter> CostCenters { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<CustomerAddressDetail> CustomerAddressDetails { get; set; }

    public virtual DbSet<CustomerPhone> CustomerPhones { get; set; }

    public virtual DbSet<DealDescription> DealDescriptions { get; set; }

    public virtual DbSet<DealItemDetail> DealItemDetails { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Discount> Discounts { get; set; }

    public virtual DbSet<DiscountAreaMapping> DiscountAreaMappings { get; set; }

    public virtual DbSet<DiscountBranchMapping> DiscountBranchMappings { get; set; }

    public virtual DbSet<DiscountDayMapping> DiscountDayMappings { get; set; }

    public virtual DbSet<DiscountOrderModeMapping> DiscountOrderModeMappings { get; set; }

    public virtual DbSet<DiscountOrderTypeMapping> DiscountOrderTypeMappings { get; set; }

    public virtual DbSet<DiscountProductDetailMapping> DiscountProductDetailMappings { get; set; }

    public virtual DbSet<DiscountProductDetailMapping20230123> DiscountProductDetailMapping20230123s { get; set; }

    public virtual DbSet<ExpenseType> ExpenseTypes { get; set; }

    public virtual DbSet<Flavour> Flavours { get; set; }

    public virtual DbSet<Gst> Gsts { get; set; }

    public virtual DbSet<InvAdjustmentDetail> InvAdjustmentDetails { get; set; }

    public virtual DbSet<InvAdjustmentMaster> InvAdjustmentMasters { get; set; }

    public virtual DbSet<InvBatch> InvBatches { get; set; }

    public virtual DbSet<InvClosingDetail> InvClosingDetails { get; set; }

    public virtual DbSet<InvClosingMaster> InvClosingMasters { get; set; }

    public virtual DbSet<InvConsumptionBatchDetail> InvConsumptionBatchDetails { get; set; }

    public virtual DbSet<InvConsumptionBatchDetail20230113> InvConsumptionBatchDetail20230113s { get; set; }

    public virtual DbSet<InvConsumptionDetail> InvConsumptionDetails { get; set; }

    public virtual DbSet<InvConsumptionDetail20230113> InvConsumptionDetail20230113s { get; set; }

    public virtual DbSet<InvConsumptionMaster> InvConsumptionMasters { get; set; }

    public virtual DbSet<InvConsumptionMaster20230113> InvConsumptionMaster20230113s { get; set; }

    public virtual DbSet<InvDemandDetail> InvDemandDetails { get; set; }

    public virtual DbSet<InvDemandMaster> InvDemandMasters { get; set; }

    public virtual DbSet<InvEmployeeMealDetail> InvEmployeeMealDetails { get; set; }

    public virtual DbSet<InvEmployeeMealMaster> InvEmployeeMealMasters { get; set; }

    public virtual DbSet<InvGoodReceivingDetail> InvGoodReceivingDetails { get; set; }

    public virtual DbSet<InvGoodReceivingDetail20221227> InvGoodReceivingDetail20221227s { get; set; }

    public virtual DbSet<InvGoodReceivingMaster> InvGoodReceivingMasters { get; set; }

    public virtual DbSet<InvGoodReceivingReturnDetail> InvGoodReceivingReturnDetails { get; set; }

    public virtual DbSet<InvGoodReceivingReturnMaster> InvGoodReceivingReturnMasters { get; set; }

    public virtual DbSet<InvInventoryStore> InvInventoryStores { get; set; }

    public virtual DbSet<InvInventoryStore20221227> InvInventoryStore20221227s { get; set; }

    public virtual DbSet<InvInventoryStore20230113> InvInventoryStore20230113s { get; set; }

    public virtual DbSet<InvIssuanceMaster> InvIssuanceMasters { get; set; }

    public virtual DbSet<InvIssuenceDetail> InvIssuenceDetails { get; set; }

    public virtual DbSet<InvPodetail> InvPodetails { get; set; }

    public virtual DbSet<InvPomaster> InvPomasters { get; set; }

    public virtual DbSet<InvPurchaseInvoiceDetail> InvPurchaseInvoiceDetails { get; set; }

    public virtual DbSet<InvPurchaseInvoiceMaster> InvPurchaseInvoiceMasters { get; set; }

    public virtual DbSet<InvReceivingDetail> InvReceivingDetails { get; set; }

    public virtual DbSet<InvReceivingMaster> InvReceivingMasters { get; set; }

    public virtual DbSet<InvRecipeDetail> InvRecipeDetails { get; set; }

    public virtual DbSet<InvRecipeMaster> InvRecipeMasters { get; set; }

    public virtual DbSet<InvRequisitionDetail> InvRequisitionDetails { get; set; }

    public virtual DbSet<InvRequisitionMaster> InvRequisitionMasters { get; set; }

    public virtual DbSet<InvSetupUnit> InvSetupUnits { get; set; }

    public virtual DbSet<InvSetupVendor> InvSetupVendors { get; set; }

    public virtual DbSet<InvSetupVendorPoc> InvSetupVendorPocs { get; set; }

    public virtual DbSet<InvSubRecipeProductionDetail> InvSubRecipeProductionDetails { get; set; }

    public virtual DbSet<InvSubRecipeProductionMaster> InvSubRecipeProductionMasters { get; set; }

    public virtual DbSet<InvTransferDetail> InvTransferDetails { get; set; }

    public virtual DbSet<InvTransferMaster> InvTransferMasters { get; set; }

    public virtual DbSet<InvTransitDetail> InvTransitDetails { get; set; }

    public virtual DbSet<InvWastageDetail> InvWastageDetails { get; set; }

    public virtual DbSet<InvWastageMaster> InvWastageMasters { get; set; }

    public virtual DbSet<LoyaltyCard> LoyaltyCards { get; set; }

    public virtual DbSet<LoyaltyCardBalance> LoyaltyCardBalances { get; set; }

    public virtual DbSet<LoyaltyCardType> LoyaltyCardTypes { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<OrderDetail20221226> OrderDetail20221226s { get; set; }

    public virtual DbSet<OrderDetail20221228> OrderDetail20221228s { get; set; }

    public virtual DbSet<OrderDetail20230104> OrderDetail20230104s { get; set; }

    public virtual DbSet<OrderDetail20230111> OrderDetail20230111s { get; set; }

    public virtual DbSet<OrderDetailLog> OrderDetailLogs { get; set; }

    public virtual DbSet<OrderExtraCharge> OrderExtraCharges { get; set; }

    public virtual DbSet<OrderMaster> OrderMasters { get; set; }

    public virtual DbSet<OrderModeOrderSourceMapping> OrderModeOrderSourceMappings { get; set; }

    public virtual DbSet<OrderPayment> OrderPayments { get; set; }

    public virtual DbSet<OrderStatus> OrderStatuses { get; set; }

    public virtual DbSet<OrderStatusLog> OrderStatusLogs { get; set; }

    public virtual DbSet<OrderStatusModeMapping> OrderStatusModeMappings { get; set; }

    public virtual DbSet<Orderdetail202212281> Orderdetail20221228s { get; set; }

    public virtual DbSet<PayableReceivableVoucherDetail> PayableReceivableVoucherDetails { get; set; }

    public virtual DbSet<PayableReceivableVoucherMaster> PayableReceivableVoucherMasters { get; set; }

    public virtual DbSet<PaymentMode> PaymentModes { get; set; }

    public virtual DbSet<PaymentModeOrderSourceMapping> PaymentModeOrderSourceMappings { get; set; }

    public virtual DbSet<PaymentVoucherDetail> PaymentVoucherDetails { get; set; }

    public virtual DbSet<PaymentVoucherMaster> PaymentVoucherMasters { get; set; }

    public virtual DbSet<PosAction> PosActions { get; set; }

    public virtual DbSet<PosRole> PosRoles { get; set; }

    public virtual DbSet<PosRoleActionMapping> PosRoleActionMappings { get; set; }

    public virtual DbSet<Printer> Printers { get; set; }

    public virtual DbSet<PrinterDepartmentMapping> PrinterDepartmentMappings { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductCategory> ProductCategories { get; set; }

    public virtual DbSet<ProductDetail> ProductDetails { get; set; }

    public virtual DbSet<ProductDetailAvailability> ProductDetailAvailabilities { get; set; }

    public virtual DbSet<ProductDetailBranchMapping> ProductDetailBranchMappings { get; set; }

    public virtual DbSet<ProductDetailCode> ProductDetailCodes { get; set; }

    public virtual DbSet<ProductDetailOrderSourcePriceMapping> ProductDetailOrderSourcePriceMappings { get; set; }

    public virtual DbSet<ProductDetailProperty> ProductDetailProperties { get; set; }

    public virtual DbSet<ProductDetailToppingMapping> ProductDetailToppingMappings { get; set; }

    public virtual DbSet<ProductSize> ProductSizes { get; set; }

    public virtual DbSet<Province> Provinces { get; set; }

    public virtual DbSet<ReservationDetail> ReservationDetails { get; set; }

    public virtual DbSet<ReservationMaster> ReservationMasters { get; set; }

    public virtual DbSet<ReservationStatus> ReservationStatuses { get; set; }

    public virtual DbSet<Rider> Riders { get; set; }

    public virtual DbSet<SalesReturnDetail> SalesReturnDetails { get; set; }

    public virtual DbSet<SalesReturnMaster> SalesReturnMasters { get; set; }

    public virtual DbSet<SetupBank> SetupBanks { get; set; }

    public virtual DbSet<SetupBankDetail> SetupBankDetails { get; set; }

    public virtual DbSet<SetupCompany> SetupCompanies { get; set; }

    public virtual DbSet<SetupCompanySetting> SetupCompanySettings { get; set; }

    public virtual DbSet<SetupExtraCharge> SetupExtraCharges { get; set; }

    public virtual DbSet<SetupFeature> SetupFeatures { get; set; }

    public virtual DbSet<SetupMaster> SetupMasters { get; set; }

    public virtual DbSet<SetupMasterDetail> SetupMasterDetails { get; set; }

    public virtual DbSet<SetupMenuItem> SetupMenuItems { get; set; }

    public virtual DbSet<SetupMenuItemFeatureMapping> SetupMenuItemFeatureMappings { get; set; }

    public virtual DbSet<SetupProductTag> SetupProductTags { get; set; }

    public virtual DbSet<SetupRoleAccess> SetupRoleAccesses { get; set; }

    public virtual DbSet<SetupRoleAccessAction> SetupRoleAccessActions { get; set; }

    public virtual DbSet<SetupRoleMenuItemFeatureMapping> SetupRoleMenuItemFeatureMappings { get; set; }

    public virtual DbSet<SetupTypeDetail> SetupTypeDetails { get; set; }

    public virtual DbSet<SetupTypeMaster> SetupTypeMasters { get; set; }

    public virtual DbSet<Shift> Shifts { get; set; }

    public virtual DbSet<ShiftDetail> ShiftDetails { get; set; }

    public virtual DbSet<Table> Tables { get; set; }

    public virtual DbSet<TableMerge> TableMerges { get; set; }

    public virtual DbSet<TableMergeDetail> TableMergeDetails { get; set; }

    public virtual DbSet<TblFiscalMonth> TblFiscalMonths { get; set; }

    public virtual DbSet<TblFiscalYear> TblFiscalYears { get; set; }

    public virtual DbSet<TblGetorderList> TblGetorderLists { get; set; }

    public virtual DbSet<TblPayOff> TblPayOffs { get; set; }

    public virtual DbSet<TblPocCostCenter> TblPocCostCenters { get; set; }

    public virtual DbSet<TempOrderDetail> TempOrderDetails { get; set; }

    public virtual DbSet<TempOrderMaster> TempOrderMasters { get; set; }

    public virtual DbSet<Template> Templates { get; set; }

    public virtual DbSet<Terminal> Terminals { get; set; }

    public virtual DbSet<TerminalDetail> TerminalDetails { get; set; }

    public virtual DbSet<UserBranchMapping> UserBranchMappings { get; set; }

    public virtual DbSet<UserLogin> UserLogins { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<VendorProductDetailMapping> VendorProductDetailMappings { get; set; }

    public virtual DbSet<VwGrn> VwGrns { get; set; }

    public virtual DbSet<Waiter> Waiters { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Area>(entity =>
        {
            entity.ToTable("Area");

            entity.Property(e => e.AreaName).HasMaxLength(200);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsEnable).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(100)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.City).WithMany(p => p.Areas)
                .HasForeignKey(d => d.CityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Area_City");

            entity.HasOne(d => d.Company).WithMany(p => p.Areas)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("FK_Area_SetupCompany");
        });

        modelBuilder.Entity<AreaK2g>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Area_K2G");

            entity.Property(e => e.AreaName).HasMaxLength(200);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DangerZone).HasMaxLength(255);
            entity.Property(e => e.IsPosdata).HasColumnName("IsPOSData");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.OmsareaId).HasColumnName("OMSAreaId");
            entity.Property(e => e.UserIp)
                .HasMaxLength(100)
                .HasColumnName("UserIP");
        });

        modelBuilder.Entity<BranchDetail>(entity =>
        {
            entity.ToTable("BranchDetail");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsEnabled).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(100)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.AlternateBranch1Navigation).WithMany(p => p.BranchDetailAlternateBranch1Navigations)
                .HasForeignKey(d => d.AlternateBranch1)
                .HasConstraintName("FK_BranchDetail_BranchMaster1");

            entity.HasOne(d => d.AlternateBranch2Navigation).WithMany(p => p.BranchDetailAlternateBranch2Navigations)
                .HasForeignKey(d => d.AlternateBranch2)
                .HasConstraintName("FK_BranchDetail_BranchMaster2");

            entity.HasOne(d => d.AlternateBranch3Navigation).WithMany(p => p.BranchDetailAlternateBranch3Navigations)
                .HasForeignKey(d => d.AlternateBranch3)
                .HasConstraintName("FK_BranchDetail_BranchMaster3");

            entity.HasOne(d => d.Area).WithMany(p => p.BranchDetails)
                .HasForeignKey(d => d.AreaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BranchDetail_Area");

            entity.HasOne(d => d.Branch).WithMany(p => p.BranchDetailBranches)
                .HasForeignKey(d => d.BranchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BranchDetail_BranchMaster");
        });

        modelBuilder.Entity<BranchDetailK2g>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("BranchDetail_K2G");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(100)
                .HasColumnName("UserIP");
        });

        modelBuilder.Entity<BranchMaster>(entity =>
        {
            entity.HasKey(e => e.BranchId);

            entity.ToTable("BranchMaster");

            entity.Property(e => e.BranchName).HasMaxLength(200);
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Ntnname)
                .HasMaxLength(50)
                .HasColumnName("NTNName");
            entity.Property(e => e.Ntnnumber)
                .HasMaxLength(50)
                .HasColumnName("NTNNumber");
            entity.Property(e => e.UserIp)
                .HasMaxLength(100)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.City).WithMany(p => p.BranchMasters)
                .HasForeignKey(d => d.CityId)
                .HasConstraintName("FK_BranchMaster_Setup_MasterDetail");

            entity.HasOne(d => d.Company).WithMany(p => p.BranchMasters)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BranchMaster_SetupCompany");
        });

        modelBuilder.Entity<BranchOrderSourceMapping>(entity =>
        {
            entity.HasKey(e => e.MappingId);

            entity.ToTable("BranchOrderSourceMapping");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Branch).WithMany(p => p.BranchOrderSourceMappings)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("FK_BranchOrderSourceMapping_BranchMaster");

            entity.HasOne(d => d.OrderSource).WithMany(p => p.BranchOrderSourceMappings)
                .HasForeignKey(d => d.OrderSourceId)
                .HasConstraintName("FK_BranchOrderSourceMapping_Setup_MasterDetail");
        });

        modelBuilder.Entity<BulkRetail20221014>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("BulkRetail_20221014");

            entity.Property(e => e.Sku).HasColumnName("SKU");
        });

        modelBuilder.Entity<BusinessDay>(entity =>
        {
            entity.ToTable("BusinessDay");

            entity.Property(e => e.CloseDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.OpenDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(50)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.Branch).WithMany(p => p.BusinessDays)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("FK_BusinessDay_BranchMaster");
        });

        modelBuilder.Entity<ChartOfAccount>(entity =>
        {
            entity.ToTable("ChartOfAccount");

            entity.Property(e => e.AccountCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.NatureOfAccountId).HasColumnName("NatureOfAccountID");
            entity.Property(e => e.OpeningBalance).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UserIp)
                .HasMaxLength(50)
                .HasColumnName("UserIP");
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.ToTable("City");

            entity.Property(e => e.CityName).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(100)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.Country).WithMany(p => p.Cities)
                .HasForeignKey(d => d.CountryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_City_Country");

            entity.HasOne(d => d.Province).WithMany(p => p.Cities)
                .HasForeignKey(d => d.ProvinceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_City_Province");
        });

        modelBuilder.Entity<CloseInventoryDetail2023>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("CloseInventoryDetail_2023");
        });

        modelBuilder.Entity<CompanyPocDetail>(entity =>
        {
            entity.HasKey(e => e.PocId);

            entity.ToTable("CompanyPocDetail");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp).HasColumnName("UserIP");

            entity.HasOne(d => d.Company).WithMany(p => p.CompanyPocDetails)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("FK_CompanyPocDetail_SetupCompany");
        });

        modelBuilder.Entity<ComplainCategory>(entity =>
        {
            entity.ToTable("ComplainCategory");

            entity.Property(e => e.ComplainCategoryName).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(100)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.Company).WithMany(p => p.ComplainCategories)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("FK_ComplainCategory_SetupCompany");

            entity.HasOne(d => d.ComplainType).WithMany(p => p.ComplainCategories)
                .HasForeignKey(d => d.ComplainTypeId)
                .HasConstraintName("FK_ComplainCategory_Setup_MasterDetail");
        });

        modelBuilder.Entity<ComplainDetail>(entity =>
        {
            entity.ToTable("ComplainDetail");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(100)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.ComplainMaster).WithMany(p => p.ComplainDetails)
                .HasForeignKey(d => d.ComplainMasterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ComplainDetail_ComplainMaster");

            entity.HasOne(d => d.ComplainStatus).WithMany(p => p.ComplainDetails)
                .HasForeignKey(d => d.ComplainStatusId)
                .HasConstraintName("FK_ComplainDetail_ComplainStatus");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ComplainDetails)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ComplainDetail_UserLogin");
        });

        modelBuilder.Entity<ComplainMaster>(entity =>
        {
            entity.ToTable("ComplainMaster");

            entity.Property(e => e.ComplainNumber).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(100)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.Company).WithMany(p => p.ComplainMasters)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ComplainMaster_SetupCompany");

            entity.HasOne(d => d.ComplainCategory).WithMany(p => p.ComplainMasters)
                .HasForeignKey(d => d.ComplainCategoryId)
                .HasConstraintName("FK_ComplainMaster_ComplainCategory");

            entity.HasOne(d => d.ComplainStatus).WithMany(p => p.ComplainMasters)
                .HasForeignKey(d => d.ComplainStatusId)
                .HasConstraintName("FK_ComplainMaster_ComplainStatus");

            entity.HasOne(d => d.ComplainType).WithMany(p => p.ComplainMasters)
                .HasForeignKey(d => d.ComplainTypeId)
                .HasConstraintName("FK_ComplainMaster_ComplainType");

            entity.HasOne(d => d.OrderMaster).WithMany(p => p.ComplainMasters)
                .HasForeignKey(d => d.OrderMasterId)
                .HasConstraintName("FK_ComplainMaster_OrderMaster");
        });

        modelBuilder.Entity<ComplainStatus>(entity =>
        {
            entity.ToTable("ComplainStatus");

            entity.Property(e => e.ComplainStatusId).ValueGeneratedNever();
            entity.Property(e => e.ComplainStatusName).HasMaxLength(200);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsClosed).HasDefaultValue(false);
            entity.Property(e => e.IsInitial).HasDefaultValue(false);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(100)
                .HasColumnName("UserIP");
        });

        modelBuilder.Entity<CostCenter>(entity =>
        {
            entity.ToTable("CostCenter");

            entity.Property(e => e.CostCenterCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CostCenterName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK_CostCenter_CostCenter1");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.ToTable("Country");

            entity.Property(e => e.CountryName).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(100)
                .HasColumnName("UserIP");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("Customer");

            entity.Property(e => e.Cnic).HasColumnName("CNIC");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Gst)
                .HasMaxLength(50)
                .HasColumnName("GST");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Ntn)
                .HasMaxLength(50)
                .HasColumnName("NTN");
            entity.Property(e => e.Sst)
                .HasMaxLength(50)
                .HasColumnName("SST");
            entity.Property(e => e.Title).HasMaxLength(50);
            entity.Property(e => e.UserIp)
                .HasMaxLength(50)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.Company).WithMany(p => p.Customers)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("FK_Customer_SetupCompany");

            entity.HasOne(d => d.Phone).WithMany(p => p.Customers)
                .HasForeignKey(d => d.PhoneId)
                .HasConstraintName("FK_Customer_CustomerPhone");
        });

        modelBuilder.Entity<CustomerAddressDetail>(entity =>
        {
            entity.HasKey(e => e.CustomerAddressId);

            entity.ToTable("CustomerAddressDetail");

            entity.Property(e => e.BlockFloor).HasMaxLength(50);
            entity.Property(e => e.Building).HasMaxLength(200);
            entity.Property(e => e.CompanyName).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LandMark).HasMaxLength(200);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.RoomHouse).HasMaxLength(50);
            entity.Property(e => e.StreetRowLane).HasMaxLength(100);
            entity.Property(e => e.UserIp)
                .HasMaxLength(100)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.AddressType).WithMany(p => p.CustomerAddressDetailAddressTypes)
                .HasForeignKey(d => d.AddressTypeId)
                .HasConstraintName("FK_CustomerAddressDetail_Setup_MasterDetail");

            entity.HasOne(d => d.Area).WithMany(p => p.CustomerAddressDetails)
                .HasForeignKey(d => d.AreaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CustomerAddressDetail_Area");

            entity.HasOne(d => d.Caption).WithMany(p => p.CustomerAddressDetailCaptions)
                .HasForeignKey(d => d.CaptionId)
                .HasConstraintName("FK_CustomerAddressDetail_Setup_MasterDetail1");

            entity.HasOne(d => d.City).WithMany(p => p.CustomerAddressDetails)
                .HasForeignKey(d => d.CityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CustomerAddressDetail_City");

            entity.HasOne(d => d.Company).WithMany(p => p.CustomerAddressDetails)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CustomerAddressDetail_SetupCompany");

            entity.HasOne(d => d.Phone).WithMany(p => p.CustomerAddressDetails)
                .HasForeignKey(d => d.PhoneId)
                .HasConstraintName("FK_CustomerAddressDetail_CustomerPhone");
        });

        modelBuilder.Entity<CustomerPhone>(entity =>
        {
            entity.HasKey(e => e.PhoneId).HasName("PK_CustomerPhoneDetail");

            entity.ToTable("CustomerPhone");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.PhoneNumber).HasMaxLength(50);
            entity.Property(e => e.UserIp)
                .HasMaxLength(100)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.Company).WithMany(p => p.CustomerPhones)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("FK_CustomerPhone_SetupCompany");

            entity.HasOne(d => d.PhoneType).WithMany(p => p.CustomerPhones)
                .HasForeignKey(d => d.PhoneTypeId)
                .HasConstraintName("FK_CustomerPhone_Setup_MasterDetail");
        });

        modelBuilder.Entity<DealDescription>(entity =>
        {
            entity.HasKey(e => e.DealDescId);

            entity.ToTable("DealDescription");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Price).HasDefaultValue(0.0);

            entity.HasOne(d => d.DealItem).WithMany(p => p.DealDescriptions)
                .HasForeignKey(d => d.DealItemId)
                .HasConstraintName("FK_DealDescription_DealItemDetail");

            entity.HasOne(d => d.ProductDetail).WithMany(p => p.DealDescriptions)
                .HasForeignKey(d => d.ProductDetailId)
                .HasConstraintName("FK_DealDescription_ProductDetail");
        });

        modelBuilder.Entity<DealItemDetail>(entity =>
        {
            entity.HasKey(e => e.DealItemId);

            entity.ToTable("DealItemDetail");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(100)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.ProductDetail).WithMany(p => p.DealItemDetails)
                .HasForeignKey(d => d.ProductDetailId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DealItemDetail_ProductDetail");

            entity.HasOne(d => d.ProductProperty).WithMany(p => p.DealItemDetails)
                .HasForeignKey(d => d.ProductPropertyId)
                .HasConstraintName("FK_DealItemDetail_Setup_MasterDetail");

            entity.HasOne(d => d.Size).WithMany(p => p.DealItemDetails)
                .HasForeignKey(d => d.SizeId)
                .HasConstraintName("FK_DealItemDetail_ProductSize");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.ToTable("Department");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DepartmentName).HasMaxLength(100);
            entity.Property(e => e.IsEnable).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(100)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.Company).WithMany(p => p.Departments)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("FK_Department_SetupCompany");
        });

        modelBuilder.Entity<Discount>(entity =>
        {
            entity.ToTable("Discount");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DiscountCapEnd).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DiscountCapStart).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DiscountName).HasMaxLength(50);
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsActiveInMobile).HasDefaultValue(false);
            entity.Property(e => e.IsActiveInOdms)
                .HasDefaultValue(false)
                .HasColumnName("IsActiveInODMS");
            entity.Property(e => e.IsActiveInPos).HasColumnName("IsActiveInPOS");
            entity.Property(e => e.IsPercentage).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Priority).HasDefaultValue(1);
            entity.Property(e => e.StartDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(100)
                .HasColumnName("UserIP");
            entity.Property(e => e.VocherCode).HasMaxLength(50);

            entity.HasOne(d => d.Company).WithMany(p => p.Discounts)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("FK_Discount_SetupCompany");
        });

        modelBuilder.Entity<DiscountAreaMapping>(entity =>
        {
            entity.ToTable("DiscountAreaMapping");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(100)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.Area).WithMany(p => p.DiscountAreaMappings)
                .HasForeignKey(d => d.AreaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DiscountAreaMapping_Area");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.DiscountAreaMappingCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_DiscountAreaMapping_UserLogin");

            entity.HasOne(d => d.Discount).WithMany(p => p.DiscountAreaMappings)
                .HasForeignKey(d => d.DiscountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DiscountAreaMapping_Discount");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.DiscountAreaMappingModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("FK_DiscountAreaMapping_UserLogin1");
        });

        modelBuilder.Entity<DiscountBranchMapping>(entity =>
        {
            entity.ToTable("DiscountBranchMapping");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(100)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.Branch).WithMany(p => p.DiscountBranchMappings)
                .HasForeignKey(d => d.BranchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DiscountBranchMapping_BranchMaster");
        });

        modelBuilder.Entity<DiscountDayMapping>(entity =>
        {
            entity.ToTable("DiscountDayMapping");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(100)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.DiscountDayMappingCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_DiscountDayMapping_UserLogin");

            entity.HasOne(d => d.Day).WithMany(p => p.DiscountDayMappings)
                .HasForeignKey(d => d.DayId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DiscountDayMapping_Setup_MasterDetail");

            entity.HasOne(d => d.Discount).WithMany(p => p.DiscountDayMappings)
                .HasForeignKey(d => d.DiscountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DiscountDayMapping_Discount");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.DiscountDayMappingModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("FK_DiscountDayMapping_UserLogin1");
        });

        modelBuilder.Entity<DiscountOrderModeMapping>(entity =>
        {
            entity.ToTable("DiscountOrderModeMapping");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(100)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.DiscountOrderModeMappingCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_DiscountOrderModeMapping_UserLogin");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.DiscountOrderModeMappingModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("FK_DiscountOrderModeMapping_UserLogin1");

            entity.HasOne(d => d.OrderMode).WithMany(p => p.DiscountOrderModeMappings)
                .HasForeignKey(d => d.OrderModeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DiscountOrderModeMapping_Setup_MasterDetail");
        });

        modelBuilder.Entity<DiscountOrderTypeMapping>(entity =>
        {
            entity.ToTable("DiscountOrderTypeMapping");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(100)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.DiscountOrderTypeMappingCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_DiscountOrderTypeMapping_UserLogin");

            entity.HasOne(d => d.Discount).WithMany(p => p.DiscountOrderTypeMappings)
                .HasForeignKey(d => d.DiscountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DiscountOrderTypeMapping_Discount");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.DiscountOrderTypeMappingModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("FK_DiscountOrderTypeMapping_UserLogin1");

            entity.HasOne(d => d.OrderType).WithMany(p => p.DiscountOrderTypeMappings)
                .HasForeignKey(d => d.OrderTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DiscountOrderTypeMapping_Setup_MasterDetail");
        });

        modelBuilder.Entity<DiscountProductDetailMapping>(entity =>
        {
            entity.ToTable("DiscountProductDetailMapping");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(100)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.DiscountProductDetailMappingCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_DiscountProductDetailMapping_UserLogin");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.DiscountProductDetailMappingModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("FK_DiscountProductDetailMapping_UserLogin1");

            entity.HasOne(d => d.ProductDetail).WithMany(p => p.DiscountProductDetailMappings)
                .HasForeignKey(d => d.ProductDetailId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DiscountProductDetailMapping_ProductDetail");
        });

        modelBuilder.Entity<DiscountProductDetailMapping20230123>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("DiscountProductDetailMapping_20230123");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DiscountProductDetailMappingId).ValueGeneratedOnAdd();
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(100)
                .HasColumnName("UserIP");
        });

        modelBuilder.Entity<ExpenseType>(entity =>
        {
            entity.ToTable("ExpenseType");

            entity.Property(e => e.ExpenseTypeId).HasColumnName("ExpenseTypeID");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ExpenseTypeName).HasMaxLength(50);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(50)
                .HasColumnName("UserIP");
        });

        modelBuilder.Entity<Flavour>(entity =>
        {
            entity.ToTable("Flavour");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.FlavourName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(100)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.Company).WithMany(p => p.Flavours)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("FK_Flavour_SetupCompany");
        });

        modelBuilder.Entity<Gst>(entity =>
        {
            entity.ToTable("GST");

            entity.Property(e => e.Gstid).HasColumnName("GSTId");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Gstname).HasColumnName("GSTName");
            entity.Property(e => e.Gstpercentage).HasColumnName("GSTPercentage");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(255)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.City).WithMany(p => p.Gsts)
                .HasForeignKey(d => d.CityId)
                .HasConstraintName("FK_GST_City");

            entity.HasOne(d => d.Company).WithMany(p => p.Gsts)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("FK_GST_SetupCompany");

            entity.HasOne(d => d.PaymentMode).WithMany(p => p.Gsts)
                .HasForeignKey(d => d.PaymentModeId)
                .HasConstraintName("FK_GST_PaymentMode");
        });

        modelBuilder.Entity<InvAdjustmentDetail>(entity =>
        {
            entity.HasKey(e => e.InvAdjustmentDetailId).HasName("PK_Inv_AdjustmentDetail_1");

            entity.ToTable("Inv_AdjustmentDetail");

            entity.Property(e => e.InvAdjustmentDetailId).HasColumnName("InvAdjustmentDetailID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.InvAdjustmentId).HasColumnName("InvAdjustmentID");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Level1UnitId).HasColumnName("Level1UnitID");
            entity.Property(e => e.Level2UnitId).HasColumnName("Level2UnitID");
            entity.Property(e => e.Level3UnitId).HasColumnName("Level3UnitID");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(100)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.Batch).WithMany(p => p.InvAdjustmentDetails)
                .HasForeignKey(d => d.BatchId)
                .HasConstraintName("FK_Inv_AdjustmentDetail_Inv_AdjustmentMaster1");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.InvAdjustmentDetailCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Inv_AdjustmentDetail_UserLogin1");

            entity.HasOne(d => d.InvAdjustment).WithMany(p => p.InvAdjustmentDetails)
                .HasForeignKey(d => d.InvAdjustmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Inv_AdjustmentDetail_Inv_AdjustMaster1");

            entity.HasOne(d => d.Level1Unit).WithMany(p => p.InvAdjustmentDetailLevel1Units)
                .HasForeignKey(d => d.Level1UnitId)
                .HasConstraintName("FK_Inv_AdjustmentDetail_Inv_SetupUnit");

            entity.HasOne(d => d.Level2Unit).WithMany(p => p.InvAdjustmentDetailLevel2Units)
                .HasForeignKey(d => d.Level2UnitId)
                .HasConstraintName("FK_Inv_AdjustmentDetail_Inv_SetupUnit1");

            entity.HasOne(d => d.Level3Unit).WithMany(p => p.InvAdjustmentDetailLevel3Units)
                .HasForeignKey(d => d.Level3UnitId)
                .HasConstraintName("FK_Inv_AdjustmentDetail_Inv_SetupUnit2");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.InvAdjustmentDetailModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("FK_Inv_AdjustmentDetail_UserLogin");

            entity.HasOne(d => d.ProductDetail).WithMany(p => p.InvAdjustmentDetails)
                .HasForeignKey(d => d.ProductDetailId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Inv_AdjustmentDetail_ProductDetail");

            entity.HasOne(d => d.Type).WithMany(p => p.InvAdjustmentDetails)
                .HasForeignKey(d => d.TypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Inv_AdjustmentDetail_Setup_MasterDetail");
        });

        modelBuilder.Entity<InvAdjustmentMaster>(entity =>
        {
            entity.HasKey(e => e.InvAdjustmentId).HasName("PK_Inv_InventoryAdjustment");

            entity.ToTable("Inv_AdjustmentMaster");

            entity.Property(e => e.InvAdjustmentId).HasColumnName("InvAdjustmentID");
            entity.Property(e => e.AdjustmentNo).IsUnicode(false);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.InvAdjustmentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(100)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.Branch).WithMany(p => p.InvAdjustmentMasters)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("FK_Inv_AdjustmentMaster_BranchMaster1");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.InvAdjustmentMasterCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Inv_AdjustmentMaster_UserLogin1");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.InvAdjustmentMasterModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("FK_Inv_AdjustmentMaster_UserLogin2");
        });

        modelBuilder.Entity<InvBatch>(entity =>
        {
            entity.HasKey(e => e.BatchId).HasName("PK_Batch");

            entity.ToTable("Inv_Batch");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ExpiryDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ManufactureDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp).HasColumnName("UserIP");

            entity.HasOne(d => d.Company).WithMany(p => p.InvBatches)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("FK_Inv_Batch_SetupCompany");

            entity.HasOne(d => d.ProductDetail).WithMany(p => p.InvBatches)
                .HasForeignKey(d => d.ProductDetailId)
                .HasConstraintName("FK_Inv_Batch_ProductDetail");

            entity.HasOne(d => d.ProductionDetail).WithMany(p => p.InvBatches)
                .HasForeignKey(d => d.ProductionDetailId)
                .HasConstraintName("FK_Inv_Batch_Inv_SubRecipeProductionDetail");
        });

        modelBuilder.Entity<InvClosingDetail>(entity =>
        {
            entity.HasKey(e => e.CloseDetailId);

            entity.ToTable("Inv_ClosingDetail");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(50)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.Batch).WithMany(p => p.InvClosingDetails)
                .HasForeignKey(d => d.BatchId)
                .HasConstraintName("FK_Inv_ClosingDetail_Inv_Batch");

            entity.HasOne(d => d.Close).WithMany(p => p.InvClosingDetails)
                .HasForeignKey(d => d.CloseId)
                .HasConstraintName("FK_Inv_ClosingDetail_Inv_ClosingMaster");

            entity.HasOne(d => d.ConsumeUnit).WithMany(p => p.InvClosingDetailConsumeUnits)
                .HasForeignKey(d => d.ConsumeUnitId)
                .HasConstraintName("FK_Inv_ClosingDetail_Inv_SetupUnit");

            entity.HasOne(d => d.IssueUnit).WithMany(p => p.InvClosingDetailIssueUnits)
                .HasForeignKey(d => d.IssueUnitId)
                .HasConstraintName("FK_Inv_ClosingDetail_Inv_SetupUnit2");

            entity.HasOne(d => d.ProductDetail).WithMany(p => p.InvClosingDetails)
                .HasForeignKey(d => d.ProductDetailId)
                .HasConstraintName("FK_Inv_ClosingDetail_ProductDetail");

            entity.HasOne(d => d.PurchaseUnit).WithMany(p => p.InvClosingDetailPurchaseUnits)
                .HasForeignKey(d => d.PurchaseUnitId)
                .HasConstraintName("FK_Inv_ClosingDetail_Inv_SetupUnit1");
        });

        modelBuilder.Entity<InvClosingMaster>(entity =>
        {
            entity.HasKey(e => e.CloseId);

            entity.ToTable("Inv_ClosingMaster");

            entity.Property(e => e.ClosingDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(50)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.Branch).WithMany(p => p.InvClosingMasters)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("FK_Inv_ClosingMaster_BranchMaster");
        });

        modelBuilder.Entity<InvConsumptionBatchDetail>(entity =>
        {
            entity.HasKey(e => e.ConsumptionBatchDetailId);

            entity.ToTable("Inv_ConsumptionBatchDetail");

            entity.HasIndex(e => e.ConsumptionId, "Varirance_report_index");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(50)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.Batch).WithMany(p => p.InvConsumptionBatchDetails)
                .HasForeignKey(d => d.BatchId)
                .HasConstraintName("FK_Inv_ConsumptionBatchDetail_Inv_Batch");

            entity.HasOne(d => d.Consumption).WithMany(p => p.InvConsumptionBatchDetails)
                .HasForeignKey(d => d.ConsumptionId)
                .HasConstraintName("FK_Inv_ConsumptionBatchDetail_Inv_ConsumptionMaster");

            entity.HasOne(d => d.ProductDetail).WithMany(p => p.InvConsumptionBatchDetails)
                .HasForeignKey(d => d.ProductDetailId)
                .HasConstraintName("FK_Inv_ConsumptionBatchDetail_ProductDetail");
        });

        modelBuilder.Entity<InvConsumptionBatchDetail20230113>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Inv_ConsumptionBatchDetail_20230113");

            entity.Property(e => e.ConsumptionBatchDetailId).ValueGeneratedOnAdd();
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(50)
                .HasColumnName("UserIP");
        });

        modelBuilder.Entity<InvConsumptionDetail>(entity =>
        {
            entity.HasKey(e => e.ConsumptionDetailId);

            entity.ToTable("Inv_ConsumptionDetail");

            entity.HasIndex(e => new { e.ConsumptionId, e.IsActive }, "Consumption_index1");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(50)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.Consumption).WithMany(p => p.InvConsumptionDetails)
                .HasForeignKey(d => d.ConsumptionId)
                .HasConstraintName("FK_Inv_ConsumptionDetail_Inv_ConsumptionMaster");

            entity.HasOne(d => d.ProductDetail).WithMany(p => p.InvConsumptionDetails)
                .HasForeignKey(d => d.ProductDetailId)
                .HasConstraintName("FK_Inv_ConsumptionDetail_ProductDetail");
        });

        modelBuilder.Entity<InvConsumptionDetail20230113>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Inv_ConsumptionDetail_20230113");

            entity.Property(e => e.ConsumptionDetailId).ValueGeneratedOnAdd();
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(50)
                .HasColumnName("UserIP");
        });

        modelBuilder.Entity<InvConsumptionMaster>(entity =>
        {
            entity.HasKey(e => e.ConsumptionId);

            entity.ToTable("Inv_ConsumptionMaster");

            entity.HasIndex(e => new { e.BranchId, e.EmployeeMealMasterId }, "Variance_Report_Index");

            entity.Property(e => e.BranchId).HasColumnName("BranchID");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(50)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.Branch).WithMany(p => p.InvConsumptionMasters)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("FK_Inv_ConsumptionMaster_BranchMaster");

            entity.HasOne(d => d.EmployeeMealMaster).WithMany(p => p.InvConsumptionMasters)
                .HasForeignKey(d => d.EmployeeMealMasterId)
                .HasConstraintName("FK_Inv_ConsumptionMaster_Inv_EmployeeMealMaster");

            entity.HasOne(d => d.Production).WithMany(p => p.InvConsumptionMasters)
                .HasForeignKey(d => d.ProductionId)
                .HasConstraintName("FK_Inv_ConsumptionMaster_Inv_SubRecipeProductionMaster");

            entity.HasOne(d => d.Recipe).WithMany(p => p.InvConsumptionMasters)
                .HasForeignKey(d => d.RecipeId)
                .HasConstraintName("FK_Inv_ConsumptionMaster_Inv_RecipeMaster");
        });

        modelBuilder.Entity<InvConsumptionMaster20230113>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Inv_ConsumptionMaster_20230113");

            entity.Property(e => e.BranchId).HasColumnName("BranchID");
            entity.Property(e => e.ConsumptionId).ValueGeneratedOnAdd();
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(50)
                .HasColumnName("UserIP");
        });

        modelBuilder.Entity<InvDemandDetail>(entity =>
        {
            entity.HasKey(e => e.DemandDetailId).HasName("PK__Inv_Dema__6345EA3ADD2E6D19");

            entity.ToTable("Inv_DemandDetail");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp).HasColumnName("UserIP");

            entity.HasOne(d => d.DemandMaster).WithMany(p => p.InvDemandDetails)
                .HasForeignKey(d => d.DemandMasterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Inv_DemandDetail_Inv_DemandMaster1");

            entity.HasOne(d => d.DemandUnitIdInConsumeNavigation).WithMany(p => p.InvDemandDetailDemandUnitIdInConsumeNavigations)
                .HasForeignKey(d => d.DemandUnitIdInConsume)
                .HasConstraintName("FK_Inv_DemandDetail_Inv_DemandMaster");

            entity.HasOne(d => d.DemandUnitIdInIssueNavigation).WithMany(p => p.InvDemandDetailDemandUnitIdInIssueNavigations)
                .HasForeignKey(d => d.DemandUnitIdInIssue)
                .HasConstraintName("FK_Inv_DemandDetail_Inv_SetupUnit1");

            entity.HasOne(d => d.ProductDetail).WithMany(p => p.InvDemandDetails)
                .HasForeignKey(d => d.ProductDetailId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Inv_DemandDetail_ProductDetail");
        });

        modelBuilder.Entity<InvDemandMaster>(entity =>
        {
            entity.HasKey(e => e.DemandMasterId).HasName("PK__Inv_Dema__349DED50B244220D");

            entity.ToTable("Inv_DemandMaster");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("UserIP");
        });

        modelBuilder.Entity<InvEmployeeMealDetail>(entity =>
        {
            entity.HasKey(e => e.EmployeeMealDetailId).HasName("PK_EmployeeMealDetailId");

            entity.ToTable("Inv_EmployeeMealDetail");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(50)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.EmployeeMealMaster).WithMany(p => p.InvEmployeeMealDetails)
                .HasForeignKey(d => d.EmployeeMealMasterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Inv_EmployeeMealDetail_Inv_EmployeeMealMaster");

            entity.HasOne(d => d.Level3Unit).WithMany(p => p.InvEmployeeMealDetails)
                .HasForeignKey(d => d.Level3UnitId)
                .HasConstraintName("FK_Inv_EmployeeMealDetail_Inv_SetupUnit");

            entity.HasOne(d => d.ProductDetail).WithMany(p => p.InvEmployeeMealDetails)
                .HasForeignKey(d => d.ProductDetailId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Inv_EmployeeMealDetail_ProductDetail");
        });

        modelBuilder.Entity<InvEmployeeMealMaster>(entity =>
        {
            entity.HasKey(e => e.EmployeeMealMasterId).HasName("PK_EmployeeMealMasterId");

            entity.ToTable("Inv_EmployeeMealMaster");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.EmployeeMealNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.Branch).WithMany(p => p.InvEmployeeMealMasters)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("FK_Inv_EmployeeMealMaster_BranchMaster");

            entity.HasOne(d => d.Company).WithMany(p => p.InvEmployeeMealMasters)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Inv_EmployeeMealMaster_SetupCompany");
        });

        modelBuilder.Entity<InvGoodReceivingDetail>(entity =>
        {
            entity.HasKey(e => e.GoodReceivingDetailId);

            entity.ToTable("Inv_GoodReceivingDetail");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ExpiryDate).HasColumnType("datetime");
            entity.Property(e => e.ManufactureDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .IsUnicode(false)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.ConsumeUnit).WithMany(p => p.InvGoodReceivingDetailConsumeUnits)
                .HasForeignKey(d => d.ConsumeUnitId)
                .HasConstraintName("FK_Inv_GoodReceivingDetail_Inv_SetupUnit2");

            entity.HasOne(d => d.GoodReceiving).WithMany(p => p.InvGoodReceivingDetails)
                .HasForeignKey(d => d.GoodReceivingId)
                .HasConstraintName("FK_Inv_GoodReceivingDetail_Inv_GoodReceivingMaster");

            entity.HasOne(d => d.IssueUnit).WithMany(p => p.InvGoodReceivingDetailIssueUnits)
                .HasForeignKey(d => d.IssueUnitId)
                .HasConstraintName("FK_Inv_GoodReceivingDetail_Inv_SetupUnit1");

            entity.HasOne(d => d.ProductDetail).WithMany(p => p.InvGoodReceivingDetails)
                .HasForeignKey(d => d.ProductDetailId)
                .HasConstraintName("FK_Inv_GoodReceivingDetail_ProductDetail");

            entity.HasOne(d => d.PurchaseUnit).WithMany(p => p.InvGoodReceivingDetailPurchaseUnits)
                .HasForeignKey(d => d.PurchaseUnitId)
                .HasConstraintName("FK_Inv_GoodReceivingDetail_Inv_SetupUnit");
        });

        modelBuilder.Entity<InvGoodReceivingDetail20221227>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Inv_GoodReceivingDetail_20221227");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ExpiryDate).HasColumnType("datetime");
            entity.Property(e => e.GoodReceivingDetailId).ValueGeneratedOnAdd();
            entity.Property(e => e.ManufactureDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .IsUnicode(false)
                .HasColumnName("UserIP");
        });

        modelBuilder.Entity<InvGoodReceivingMaster>(entity =>
        {
            entity.HasKey(e => e.GoodReceivingId).HasName("PK_Inv_GoodReciving_Master");

            entity.ToTable("Inv_GoodReceivingMaster");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.GoodReceivingNumber).IsUnicode(false);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .IsUnicode(false)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.Branch).WithMany(p => p.InvGoodReceivingMasters)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("FK_Inv_GoodReceivingMaster_BranchMaster");

            entity.HasOne(d => d.Status).WithMany(p => p.InvGoodReceivingMasters)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK_Inv_GoodReceivingMaster_Setup_MasterDetail");

            entity.HasOne(d => d.Vendor).WithMany(p => p.InvGoodReceivingMasters)
                .HasForeignKey(d => d.VendorId)
                .HasConstraintName("FK_Inv_GoodReceivingMaster_Inv_SetupVendor");
        });

        modelBuilder.Entity<InvGoodReceivingReturnDetail>(entity =>
        {
            entity.HasKey(e => e.GoodReceivingReturnDetailId);

            entity.ToTable("Inv_GoodReceivingReturnDetail");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ExpiryDate).HasColumnType("datetime");
            entity.Property(e => e.ManufactureDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .IsUnicode(false)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.Batch).WithMany(p => p.InvGoodReceivingReturnDetails)
                .HasForeignKey(d => d.BatchId)
                .HasConstraintName("FK_Inv_GoodReceivingReturnDetail_Inv_Batch");

            entity.HasOne(d => d.ConsumeUnit).WithMany(p => p.InvGoodReceivingReturnDetailConsumeUnits)
                .HasForeignKey(d => d.ConsumeUnitId)
                .HasConstraintName("FK_Inv_GoodReceivingReturnDetail_Inv_SetupUnit2");

            entity.HasOne(d => d.GoodReceivingDetail).WithMany(p => p.InvGoodReceivingReturnDetails)
                .HasForeignKey(d => d.GoodReceivingDetailId)
                .HasConstraintName("FK_Inv_GoodReceivingReturnDetail_Inv_GoodReceivingDetail");

            entity.HasOne(d => d.GoodReceivingReturn).WithMany(p => p.InvGoodReceivingReturnDetails)
                .HasForeignKey(d => d.GoodReceivingReturnId)
                .HasConstraintName("FK_Inv_GoodReceivingReturnDetail_Inv_GoodReceivingReturnMaster");

            entity.HasOne(d => d.IssueUnit).WithMany(p => p.InvGoodReceivingReturnDetailIssueUnits)
                .HasForeignKey(d => d.IssueUnitId)
                .HasConstraintName("FK_Inv_GoodReceivingReturnDetail_Inv_SetupUnit1");

            entity.HasOne(d => d.ProductDetail).WithMany(p => p.InvGoodReceivingReturnDetails)
                .HasForeignKey(d => d.ProductDetailId)
                .HasConstraintName("FK_Inv_GoodReceivingReturnDetail_ProductDetail");

            entity.HasOne(d => d.PurchaseUnit).WithMany(p => p.InvGoodReceivingReturnDetailPurchaseUnits)
                .HasForeignKey(d => d.PurchaseUnitId)
                .HasConstraintName("FK_Inv_GoodReceivingReturnDetail_Inv_SetupUnit");
        });

        modelBuilder.Entity<InvGoodReceivingReturnMaster>(entity =>
        {
            entity.HasKey(e => e.GoodReceivingReturnId).HasName("PK_Inv_GoodReceivingReturn_Master");

            entity.ToTable("Inv_GoodReceivingReturnMaster");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.GoodReceivingReturnNumber).IsUnicode(false);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .IsUnicode(false)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.Branch).WithMany(p => p.InvGoodReceivingReturnMasters)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("FK_Inv_GoodReceivingReturnMaster_BranchMaster");

            entity.HasOne(d => d.GoodReceiving).WithMany(p => p.InvGoodReceivingReturnMasters)
                .HasForeignKey(d => d.GoodReceivingId)
                .HasConstraintName("FK_Inv_GoodReceivingReturnMaster_Inv_GoodReceivingMaster");

            entity.HasOne(d => d.Status).WithMany(p => p.InvGoodReceivingReturnMasters)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK_Inv_GoodReceivingReturnMaster_Setup_MasterDetail");

            entity.HasOne(d => d.Vendor).WithMany(p => p.InvGoodReceivingReturnMasters)
                .HasForeignKey(d => d.VendorId)
                .HasConstraintName("FK_Inv_GoodReceivingReturnMaster_Inv_SetupVendor");
        });

        modelBuilder.Entity<InvInventoryStore>(entity =>
        {
            entity.HasKey(e => e.InventoryId).HasName("PK_InventoryStore");

            entity.ToTable("Inv_InventoryStore");

            entity.HasIndex(e => new { e.TypeId, e.IsActive }, "Consumption_Index1");

            entity.HasIndex(e => new { e.ProductDetailId, e.BranchId, e.TypeId, e.IsActive, e.GoodReceivingDetailId, e.InventoryDate }, "Varirance_report_index");

            entity.HasIndex(e => new { e.ProductDetailId, e.BranchId, e.TypeId, e.InventoryDate, e.ConsumptionBatchDetailId, e.ReceivingDetailId, e.WastageDetailId }, "Varirance_report_index_1");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.GoodReceivingDetailId).HasColumnName("GoodReceivingDetailID");
            entity.Property(e => e.InventoryDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .IsUnicode(false)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.AdjustmentDetail).WithMany(p => p.InvInventoryStores)
                .HasForeignKey(d => d.AdjustmentDetailId)
                .HasConstraintName("FK_Inv_InventoryStore_Inv_AdjustmentDetail");

            entity.HasOne(d => d.Batch).WithMany(p => p.InvInventoryStores)
                .HasForeignKey(d => d.BatchId)
                .HasConstraintName("FK_Inv_InventoryStore_Inv_Batch");

            entity.HasOne(d => d.Branch).WithMany(p => p.InvInventoryStores)
                .HasForeignKey(d => d.BranchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InventoryStore_BranchMaster");

            entity.HasOne(d => d.ConsumeUnit).WithMany(p => p.InvInventoryStores)
                .HasForeignKey(d => d.ConsumeUnitId)
                .HasConstraintName("FK_Inv_InventoryStore_Inv_SetupUnit");

            entity.HasOne(d => d.ConsumptionBatchDetail).WithMany(p => p.InvInventoryStores)
                .HasForeignKey(d => d.ConsumptionBatchDetailId)
                .HasConstraintName("FK_Inv_InventoryStore_Inv_ConsumptionBatchDetail");

            entity.HasOne(d => d.ConsumptionDetail).WithMany(p => p.InvInventoryStores)
                .HasForeignKey(d => d.ConsumptionDetailId)
                .HasConstraintName("FK_Inv_InventoryStore_Inv_ConsumptionDetail");

            entity.HasOne(d => d.GoodReceivingDetail).WithMany(p => p.InvInventoryStores)
                .HasForeignKey(d => d.GoodReceivingDetailId)
                .HasConstraintName("FK_InventoryStore_Inv_GoodReceivingDetail");

            entity.HasOne(d => d.GoodReceivingReturnDetail).WithMany(p => p.InvInventoryStores)
                .HasForeignKey(d => d.GoodReceivingReturnDetailId)
                .HasConstraintName("FK_Inv_InventoryStore_Inv_GoodReceivingReturnDetail");

            entity.HasOne(d => d.IssuanceDetail).WithMany(p => p.InvInventoryStores)
                .HasForeignKey(d => d.IssuanceDetailId)
                .HasConstraintName("FK_Inv_InventoryStore_Inv_IssuenceDetail");

            entity.HasOne(d => d.ProductDetail).WithMany(p => p.InvInventoryStores)
                .HasForeignKey(d => d.ProductDetailId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InventoryStore_ProductDetail");

            entity.HasOne(d => d.ProductionDetail).WithMany(p => p.InvInventoryStores)
                .HasForeignKey(d => d.ProductionDetailId)
                .HasConstraintName("FK_Inv_InventoryStore_Inv_SubRecipeProductionDetail");

            entity.HasOne(d => d.ReceivingDetail).WithMany(p => p.InvInventoryStores)
                .HasForeignKey(d => d.ReceivingDetailId)
                .HasConstraintName("FK_Inv_InventoryStore_Inv_ReceivingDetail");

            entity.HasOne(d => d.SalesReturnDetail).WithMany(p => p.InvInventoryStores)
                .HasForeignKey(d => d.SalesReturnDetailId)
                .HasConstraintName("FK_Inv_InventoryStore_SalesReturnDetail");

            entity.HasOne(d => d.TransitDetail).WithMany(p => p.InvInventoryStores)
                .HasForeignKey(d => d.TransitDetailId)
                .HasConstraintName("FK_Inv_InventoryStore_Inv_TransitDetail");

            entity.HasOne(d => d.Type).WithMany(p => p.InvInventoryStores)
                .HasForeignKey(d => d.TypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Inv_InventoryStore_Setup_MasterDetail");

            entity.HasOne(d => d.WastageDetail).WithMany(p => p.InvInventoryStores)
                .HasForeignKey(d => d.WastageDetailId)
                .HasConstraintName("FK_Inv_InventoryStore_Inv_WastageDetail");
        });

        modelBuilder.Entity<InvInventoryStore20221227>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Inv_InventoryStore_20221227");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.GoodReceivingDetailId).HasColumnName("GoodReceivingDetailID");
            entity.Property(e => e.InventoryDate).HasColumnType("datetime");
            entity.Property(e => e.InventoryId).ValueGeneratedOnAdd();
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .IsUnicode(false)
                .HasColumnName("UserIP");
        });

        modelBuilder.Entity<InvInventoryStore20230113>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Inv_InventoryStore_20230113");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.GoodReceivingDetailId).HasColumnName("GoodReceivingDetailID");
            entity.Property(e => e.InventoryDate).HasColumnType("datetime");
            entity.Property(e => e.InventoryId).ValueGeneratedOnAdd();
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .IsUnicode(false)
                .HasColumnName("UserIP");
        });

        modelBuilder.Entity<InvIssuanceMaster>(entity =>
        {
            entity.HasKey(e => e.IssuanceMasterId).HasName("PK__Inv_Issu__B4CB4E9A90CCFFDE");

            entity.ToTable("Inv_IssuanceMaster");

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.IssuanceNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.Branch).WithMany(p => p.InvIssuanceMasters)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("FK__Inv_Issua__Branc__27B9C2CD");

            entity.HasOne(d => d.DemandMaster).WithMany(p => p.InvIssuanceMasters)
                .HasForeignKey(d => d.DemandMasterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inv_Issua__Deman__28ADE706");
        });

        modelBuilder.Entity<InvIssuenceDetail>(entity =>
        {
            entity.HasKey(e => e.IssuanceDetailId).HasName("PK__Inv_Issu__C54C574494F543CC");

            entity.ToTable("Inv_IssuenceDetail");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(50)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.Batch).WithMany(p => p.InvIssuenceDetails)
                .HasForeignKey(d => d.BatchId)
                .HasConstraintName("FK_Inv_IssuenceDetail_Inv_Batch");

            entity.HasOne(d => d.DemandDetail).WithMany(p => p.InvIssuenceDetails)
                .HasForeignKey(d => d.DemandDetailId)
                .HasConstraintName("FK_Inv_IssuenceDetail_Inv_DemandDetail");

            entity.HasOne(d => d.IssuanceMaster).WithMany(p => p.InvIssuenceDetails)
                .HasForeignKey(d => d.IssuanceMasterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inv_Issue__Issua__2B8A53B1");

            entity.HasOne(d => d.IssuanceUnitNavigation).WithMany(p => p.InvIssuenceDetails)
                .HasForeignKey(d => d.IssuanceUnit)
                .HasConstraintName("FK_Inv_IssuenceDetail_Inv_SetupUnit");

            entity.HasOne(d => d.ProductDetail).WithMany(p => p.InvIssuenceDetails)
                .HasForeignKey(d => d.ProductDetailId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Inv_IssuenceDetail_ProductDetail");
        });

        modelBuilder.Entity<InvPodetail>(entity =>
        {
            entity.HasKey(e => e.PodetailId);

            entity.ToTable("Inv_PODetail");

            entity.Property(e => e.PodetailId).HasColumnName("PODetailId");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.GrnremainingQuantity).HasColumnName("GRNRemainingQuantity");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Poid).HasColumnName("POId");
            entity.Property(e => e.UserIp)
                .IsUnicode(false)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.Batch).WithMany(p => p.InvPodetails)
                .HasForeignKey(d => d.BatchId)
                .HasConstraintName("FK_Inv_PODetail_Inv_Batch");

            entity.HasOne(d => d.ConsumeUnit).WithMany(p => p.InvPodetailConsumeUnits)
                .HasForeignKey(d => d.ConsumeUnitId)
                .HasConstraintName("FK_Inv_PODetail_Inv_SetupUnit2");

            entity.HasOne(d => d.IssueUnit).WithMany(p => p.InvPodetailIssueUnits)
                .HasForeignKey(d => d.IssueUnitId)
                .HasConstraintName("FK_Inv_PODetail_Inv_SetupUnit1");

            entity.HasOne(d => d.Po).WithMany(p => p.InvPodetails)
                .HasForeignKey(d => d.Poid)
                .HasConstraintName("FK_Inv_PODetail_Inv_POMaster");

            entity.HasOne(d => d.ProductDetail).WithMany(p => p.InvPodetails)
                .HasForeignKey(d => d.ProductDetailId)
                .HasConstraintName("FK_Inv_PODetail_ProductDetail");

            entity.HasOne(d => d.PurchaseUnit).WithMany(p => p.InvPodetailPurchaseUnits)
                .HasForeignKey(d => d.PurchaseUnitId)
                .HasConstraintName("FK_Inv_PODetail_Inv_SetupUnit");

            entity.HasOne(d => d.RequisitionDetail).WithMany(p => p.InvPodetails)
                .HasForeignKey(d => d.RequisitionDetailId)
                .HasConstraintName("FK_Inv_PODetail_Inv_RequisitionDetail");
        });

        modelBuilder.Entity<InvPomaster>(entity =>
        {
            entity.HasKey(e => e.Poid);

            entity.ToTable("Inv_POMaster");

            entity.Property(e => e.Poid).HasColumnName("POID");
            entity.Property(e => e.BranchId).HasColumnName("BranchID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.DemandId).HasColumnName("DemandID");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Ponumber)
                .HasMaxLength(100)
                .HasColumnName("PONumber");
            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.UserIp)
                .HasMaxLength(100)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.Branch).WithMany(p => p.InvPomasters)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("FK_Inv_POMaster_BranchMaster");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.InvPomasterCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Inv_POMaster_UserLogin1");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.InvPomasterModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("FK_Inv_POMaster_UserLogin2");

            entity.HasOne(d => d.Status).WithMany(p => p.InvPomasters)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK_Inv_POMaster_Setup_MasterDetail");

            entity.HasOne(d => d.User).WithMany(p => p.InvPomasterUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Inv_POMaster_UserLogin");

            entity.HasOne(d => d.Vendor).WithMany(p => p.InvPomasters)
                .HasForeignKey(d => d.VendorId)
                .HasConstraintName("FK_Inv_POMaster_Inv_SetupVendor");
        });

        modelBuilder.Entity<InvPurchaseInvoiceDetail>(entity =>
        {
            entity.HasKey(e => e.PurchaseInvoiceDetailId);

            entity.ToTable("Inv_PurchaseInvoiceDetail");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .IsUnicode(false)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.ConsumeUnit).WithMany(p => p.InvPurchaseInvoiceDetailConsumeUnits)
                .HasForeignKey(d => d.ConsumeUnitId)
                .HasConstraintName("FK_Inv_PurchaseInvoiceDetail_Inv_SetupUnit2");

            entity.HasOne(d => d.GoodReceivingDetail).WithMany(p => p.InvPurchaseInvoiceDetails)
                .HasForeignKey(d => d.GoodReceivingDetailId)
                .HasConstraintName("FK_Inv_PurchaseInvoiceDetail_Inv_GoodReceivingDetail");

            entity.HasOne(d => d.IssueUnit).WithMany(p => p.InvPurchaseInvoiceDetailIssueUnits)
                .HasForeignKey(d => d.IssueUnitId)
                .HasConstraintName("FK_Inv_PurchaseInvoiceDetail_Inv_SetupUnit1");

            entity.HasOne(d => d.ProductDetail).WithMany(p => p.InvPurchaseInvoiceDetails)
                .HasForeignKey(d => d.ProductDetailId)
                .HasConstraintName("FK_Inv_PurchaseInvoiceDetail_ProductDetail");

            entity.HasOne(d => d.PurchaseInvoice).WithMany(p => p.InvPurchaseInvoiceDetails)
                .HasForeignKey(d => d.PurchaseInvoiceId)
                .HasConstraintName("FK_Inv_PurchaseInvoiceDetail_Inv_PurchaseInvoiceMaster");

            entity.HasOne(d => d.PurchaseUnit).WithMany(p => p.InvPurchaseInvoiceDetailPurchaseUnits)
                .HasForeignKey(d => d.PurchaseUnitId)
                .HasConstraintName("FK_Inv_PurchaseInvoiceDetail_Inv_SetupUnit");
        });

        modelBuilder.Entity<InvPurchaseInvoiceMaster>(entity =>
        {
            entity.HasKey(e => e.PurchaseInvoiceId).HasName("PK_Inv_PurchaseInvoice_Master");

            entity.ToTable("Inv_PurchaseInvoiceMaster");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.PurchaseInvoiceNumber).IsUnicode(false);
            entity.Property(e => e.UserIp)
                .IsUnicode(false)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.Branch).WithMany(p => p.InvPurchaseInvoiceMasters)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("FK_Inv_PurchaseInvoiceMaster_BranchMaster");

            entity.HasOne(d => d.Vendor).WithMany(p => p.InvPurchaseInvoiceMasters)
                .HasForeignKey(d => d.VendorId)
                .HasConstraintName("FK_Inv_PurchaseInvoiceMaster_Inv_SetupVendor");
        });

        modelBuilder.Entity<InvReceivingDetail>(entity =>
        {
            entity.HasKey(e => e.ReceivingDetailId);

            entity.ToTable("Inv_ReceivingDetail");

            entity.Property(e => e.ReceivingDetailId).HasColumnName("ReceivingDetailID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Level1UnitId).HasColumnName("Level1UnitID");
            entity.Property(e => e.Level2UnitId).HasColumnName("Level2UnitID");
            entity.Property(e => e.Level3UnitId).HasColumnName("Level3UnitID");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.ReceivingId).HasColumnName("ReceivingID");
            entity.Property(e => e.TransferDetailId).HasColumnName("TransferDetailID");
            entity.Property(e => e.UserIp)
                .HasMaxLength(100)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.Batch).WithMany(p => p.InvReceivingDetails)
                .HasForeignKey(d => d.BatchId)
                .HasConstraintName("FK_Inv_ReceivingDetail_Inv_Batch");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.InvReceivingDetailCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Inv_ReceivingDetail_UserLogin1");

            entity.HasOne(d => d.IssuanceDetail).WithMany(p => p.InvReceivingDetails)
                .HasForeignKey(d => d.IssuanceDetailId)
                .HasConstraintName("FK_Inv_ReceivingDetail_Inv_IssuenceDetail");

            entity.HasOne(d => d.Level1Unit).WithMany(p => p.InvReceivingDetailLevel1Units)
                .HasForeignKey(d => d.Level1UnitId)
                .HasConstraintName("FK_Inv_ReceivingDetail_Inv_SetupUnit1");

            entity.HasOne(d => d.Level2Unit).WithMany(p => p.InvReceivingDetailLevel2Units)
                .HasForeignKey(d => d.Level2UnitId)
                .HasConstraintName("FK_Inv_ReceivingDetail_Inv_SetupUnit2");

            entity.HasOne(d => d.Level3Unit).WithMany(p => p.InvReceivingDetailLevel3Units)
                .HasForeignKey(d => d.Level3UnitId)
                .HasConstraintName("FK_Inv_ReceivingDetail_Inv_SetupUnit3");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.InvReceivingDetailModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("FK_Inv_ReceivingDetail_UserLogin");

            entity.HasOne(d => d.ProductDetail).WithMany(p => p.InvReceivingDetails)
                .HasForeignKey(d => d.ProductDetailId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Inv_ReceivingDetail_ProductDetail");

            entity.HasOne(d => d.Receiving).WithMany(p => p.InvReceivingDetails)
                .HasForeignKey(d => d.ReceivingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Inv_ReceivingDetail_Inv_ReceivingMaster");

            entity.HasOne(d => d.TransferDetail).WithMany(p => p.InvReceivingDetails)
                .HasForeignKey(d => d.TransferDetailId)
                .HasConstraintName("FK_Inv_ReceivingDetail_Inv_TransferDetail");
        });

        modelBuilder.Entity<InvReceivingMaster>(entity =>
        {
            entity.HasKey(e => e.ReceivingId);

            entity.ToTable("Inv_ReceivingMaster");

            entity.Property(e => e.ReceivingId).HasColumnName("ReceivingID");
            entity.Property(e => e.BranchId).HasColumnName("BranchID");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsSubmit).HasDefaultValue(false);
            entity.Property(e => e.IssuanceId).HasColumnName("IssuanceID");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.ReceivingNo).HasMaxLength(50);
            entity.Property(e => e.TransferId).HasColumnName("TransferID");
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.UserIp)
                .HasMaxLength(50)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.Branch).WithMany(p => p.InvReceivingMasters)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("FK_Inv_ReceivingMaster_BranchMaster");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.InvReceivingMasterCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_Inv_ReceivingMaster_UserLogin1");

            entity.HasOne(d => d.Issuance).WithMany(p => p.InvReceivingMasters)
                .HasForeignKey(d => d.IssuanceId)
                .HasConstraintName("FK_Inv_ReceivingMaster_Inv_IssuanceMaster");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.InvReceivingMasterModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("FK_Inv_ReceivingMaster_UserLogin2");

            entity.HasOne(d => d.Transfer).WithMany(p => p.InvReceivingMasters)
                .HasForeignKey(d => d.TransferId)
                .HasConstraintName("FK_Inv_ReceivingMaster_Inv_TransferMaster");

            entity.HasOne(d => d.User).WithMany(p => p.InvReceivingMasterUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Inv_ReceivingMaster_UserLogin");
        });

        modelBuilder.Entity<InvRecipeDetail>(entity =>
        {
            entity.HasKey(e => e.RecipeDetailId);

            entity.ToTable("Inv_RecipeDetail");

            entity.Property(e => e.ConsumeUnitId).HasColumnName("ConsumeUnitID");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(50)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.ConsumeUnit).WithMany(p => p.InvRecipeDetails)
                .HasForeignKey(d => d.ConsumeUnitId)
                .HasConstraintName("FK_Inv_RecipeDetail_Inv_SetupUnit");

            entity.HasOne(d => d.OrderMode).WithMany(p => p.InvRecipeDetails)
                .HasForeignKey(d => d.OrderModeId)
                .HasConstraintName("FK_Inv_RecipeDetail_Setup_MasterDetail");

            entity.HasOne(d => d.ProductDetail).WithMany(p => p.InvRecipeDetails)
                .HasForeignKey(d => d.ProductDetailId)
                .HasConstraintName("FK_Inv_RecipeDetail_ProductDetail");

            entity.HasOne(d => d.Recipe).WithMany(p => p.InvRecipeDetails)
                .HasForeignKey(d => d.RecipeId)
                .HasConstraintName("FK_Inv_RecipeDetail_Inv_RecipeMaster");
        });

        modelBuilder.Entity<InvRecipeMaster>(entity =>
        {
            entity.HasKey(e => e.RecipeId);

            entity.ToTable("Inv_RecipeMaster");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ItemCode).HasMaxLength(50);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(50)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.Company).WithMany(p => p.InvRecipeMasters)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("FK_Inv_RecipeMaster_SetupCompany");

            entity.HasOne(d => d.ProductDetail).WithMany(p => p.InvRecipeMasterProductDetails)
                .HasForeignKey(d => d.ProductDetailId)
                .HasConstraintName("FK_Inv_RecipeMaster_ProductDetail");

            entity.HasOne(d => d.SubRecipeItem).WithMany(p => p.InvRecipeMasterSubRecipeItems)
                .HasForeignKey(d => d.SubRecipeItemId)
                .HasConstraintName("FK_Inv_RecipeMaster_ProductDetail1");
        });

        modelBuilder.Entity<InvRequisitionDetail>(entity =>
        {
            entity.HasKey(e => e.RequisitionDetailId);

            entity.ToTable("Inv_RequisitionDetail");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.TotalPoquantityInConsume).HasColumnName("TotalPOQuantityInConsume");
            entity.Property(e => e.UserIp)
                .HasMaxLength(50)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.ConsumeUnit).WithMany(p => p.InvRequisitionDetailConsumeUnits)
                .HasForeignKey(d => d.ConsumeUnitId)
                .HasConstraintName("FK_Inv_RequisitionDetail_Inv_SetupUnit");

            entity.HasOne(d => d.IssueUnit).WithMany(p => p.InvRequisitionDetailIssueUnits)
                .HasForeignKey(d => d.IssueUnitId)
                .HasConstraintName("FK_Inv_RequisitionDetail_Inv_SetupUnit2");

            entity.HasOne(d => d.ProductDetail).WithMany(p => p.InvRequisitionDetails)
                .HasForeignKey(d => d.ProductDetailId)
                .HasConstraintName("FK_Inv_RequisitionDetail_ProductDetail");

            entity.HasOne(d => d.PurchaseUnit).WithMany(p => p.InvRequisitionDetailPurchaseUnits)
                .HasForeignKey(d => d.PurchaseUnitId)
                .HasConstraintName("FK_Inv_RequisitionDetail_Inv_SetupUnit1");

            entity.HasOne(d => d.Requisition).WithMany(p => p.InvRequisitionDetails)
                .HasForeignKey(d => d.RequisitionId)
                .HasConstraintName("FK_Inv_RequisitionDetail_Inv_RequisitionMaster");
        });

        modelBuilder.Entity<InvRequisitionMaster>(entity =>
        {
            entity.HasKey(e => e.RequisitionId);

            entity.ToTable("Inv_RequisitionMaster");

            entity.Property(e => e.RequisitionId).HasColumnName("RequisitionID");
            entity.Property(e => e.BranchId).HasColumnName("BranchID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.RequisitionNumber).HasMaxLength(100);
            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.UserIp)
                .HasMaxLength(100)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.Branch).WithMany(p => p.InvRequisitionMasters)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("FK_Inv_RequisitionMaster_BranchMaster");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.InvRequisitionMasterCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Inv_RequisitionMaster_UserLogin1");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.InvRequisitionMasterModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("FK_Inv_RequisitionMaster_UserLogin2");

            entity.HasOne(d => d.Status).WithMany(p => p.InvRequisitionMasters)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK_Inv_RequisitionMaster_Setup_MasterDetail");

            entity.HasOne(d => d.User).WithMany(p => p.InvRequisitionMasterUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Inv_RequisitionMaster_UserLogin");
        });

        modelBuilder.Entity<InvSetupUnit>(entity =>
        {
            entity.HasKey(e => e.UnitId);

            entity.ToTable("Inv_SetupUnit");

            entity.Property(e => e.UnitId).HasColumnName("UnitID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UnitName).HasMaxLength(50);
            entity.Property(e => e.UserIp)
                .HasMaxLength(100)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.Company).WithMany(p => p.InvSetupUnits)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("FK_Inv_SetupUnit_SetupCompany");
        });

        modelBuilder.Entity<InvSetupVendor>(entity =>
        {
            entity.HasKey(e => e.VendorId);

            entity.ToTable("Inv_SetupVendor");

            entity.Property(e => e.ContactNo).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Gst)
                .HasMaxLength(100)
                .HasColumnName("GST");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Ntn)
                .HasMaxLength(100)
                .HasColumnName("NTN");
            entity.Property(e => e.Sst)
                .HasMaxLength(100)
                .HasColumnName("SST");
            entity.Property(e => e.UserIp).HasColumnName("UserIP");
        });

        modelBuilder.Entity<InvSetupVendorPoc>(entity =>
        {
            entity.HasKey(e => e.PocId);

            entity.ToTable("Inv_SetupVendorPoc");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Vendor).WithMany(p => p.InvSetupVendorPocs)
                .HasForeignKey(d => d.VendorId)
                .HasConstraintName("FK_Inv_SetupVendorPoc_Inv_SetupVendor");
        });

        modelBuilder.Entity<InvSubRecipeProductionDetail>(entity =>
        {
            entity.HasKey(e => e.ProductionDetailId);

            entity.ToTable("Inv_SubRecipeProductionDetail");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Level1UnitId).HasColumnName("Level1UnitID");
            entity.Property(e => e.Level2UnitId).HasColumnName("Level2UnitID");
            entity.Property(e => e.Level3UnitId).HasColumnName("Level3UnitID");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(50)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.Batch).WithMany(p => p.InvSubRecipeProductionDetails)
                .HasForeignKey(d => d.BatchId)
                .HasConstraintName("FK_Inv_SubRecipeProductionDetail_Inv_Batch");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.InvSubRecipeProductionDetailCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_Inv_SubRecipeProductionDetail_UserLogin");

            entity.HasOne(d => d.Level1Unit).WithMany(p => p.InvSubRecipeProductionDetailLevel1Units)
                .HasForeignKey(d => d.Level1UnitId)
                .HasConstraintName("FK_Inv_SubRecipeProductionDetail_Inv_SetupUnit");

            entity.HasOne(d => d.Level2Unit).WithMany(p => p.InvSubRecipeProductionDetailLevel2Units)
                .HasForeignKey(d => d.Level2UnitId)
                .HasConstraintName("FK_Inv_SubRecipeProductionDetail_Inv_SetupUnit1");

            entity.HasOne(d => d.Level3Unit).WithMany(p => p.InvSubRecipeProductionDetailLevel3Units)
                .HasForeignKey(d => d.Level3UnitId)
                .HasConstraintName("FK_Inv_SubRecipeProductionDetail_Inv_SetupUnit2");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.InvSubRecipeProductionDetailModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("FK_Inv_SubRecipeProductionDetail_UserLogin1");

            entity.HasOne(d => d.ProductDetail).WithMany(p => p.InvSubRecipeProductionDetails)
                .HasForeignKey(d => d.ProductDetailId)
                .HasConstraintName("FK_Inv_SubRecipeProductionDetail_ProductDetail");

            entity.HasOne(d => d.Production).WithMany(p => p.InvSubRecipeProductionDetails)
                .HasForeignKey(d => d.ProductionId)
                .HasConstraintName("FK_Inv_SubRecipeProductionDetail_Inv_SubRecipeProductionMaster");
        });

        modelBuilder.Entity<InvSubRecipeProductionMaster>(entity =>
        {
            entity.HasKey(e => e.ProductionId).HasName("PK_SubRecipeProductionMaster");

            entity.ToTable("Inv_SubRecipeProductionMaster");

            entity.Property(e => e.BranchId).HasColumnName("BranchID");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.ProductionNumber).HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.UserIp)
                .HasMaxLength(50)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.Branch).WithMany(p => p.InvSubRecipeProductionMasters)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("FK_SubRecipeProductionMaster_BranchMaster");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.InvSubRecipeProductionMasterCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_SubRecipeProductionMaster_UserLogin1");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.InvSubRecipeProductionMasterModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("FK_SubRecipeProductionMaster_UserLogin2");

            entity.HasOne(d => d.User).WithMany(p => p.InvSubRecipeProductionMasterUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_SubRecipeProductionMaster_UserLogin");
        });

        modelBuilder.Entity<InvTransferDetail>(entity =>
        {
            entity.HasKey(e => e.TransferDetailId);

            entity.ToTable("Inv_TransferDetail");

            entity.Property(e => e.TransferDetailId).HasColumnName("TransferDetailID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Level1UnitId).HasColumnName("Level1UnitID");
            entity.Property(e => e.Level2UnitId).HasColumnName("Level2UnitID");
            entity.Property(e => e.Level3UnitId).HasColumnName("Level3UnitID");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.TransferId).HasColumnName("TransferID");
            entity.Property(e => e.UserIp)
                .HasMaxLength(100)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.Batch).WithMany(p => p.InvTransferDetails)
                .HasForeignKey(d => d.BatchId)
                .HasConstraintName("FK_Inv_TransferDetail_Inv_Batch");

            entity.HasOne(d => d.Level1Unit).WithMany(p => p.InvTransferDetailLevel1Units)
                .HasForeignKey(d => d.Level1UnitId)
                .HasConstraintName("FK_Inv_TransferDetail_Inv_SetupUnit1");

            entity.HasOne(d => d.Level2Unit).WithMany(p => p.InvTransferDetailLevel2Units)
                .HasForeignKey(d => d.Level2UnitId)
                .HasConstraintName("FK_Inv_TransferDetail_Inv_SetupUnit2");

            entity.HasOne(d => d.Level3Unit).WithMany(p => p.InvTransferDetailLevel3Units)
                .HasForeignKey(d => d.Level3UnitId)
                .HasConstraintName("FK_Inv_TransferDetail_Inv_SetupUnit3");

            entity.HasOne(d => d.ProductDetail).WithMany(p => p.InvTransferDetails)
                .HasForeignKey(d => d.ProductDetailId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Inv_TransferDetail_ProductDetail");

            entity.HasOne(d => d.Transfer).WithMany(p => p.InvTransferDetails)
                .HasForeignKey(d => d.TransferId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Inv_TransferDetail_Inv_TransferMaster");
        });

        modelBuilder.Entity<InvTransferMaster>(entity =>
        {
            entity.HasKey(e => e.TransferId);

            entity.ToTable("Inv_TransferMaster");

            entity.Property(e => e.TransferId).HasColumnName("TransferID");
            entity.Property(e => e.BranchIdfrom).HasColumnName("BranchIDFrom");
            entity.Property(e => e.BranchIdto).HasColumnName("BranchIDTo");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.ReferenceNo).HasMaxLength(50);
            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.UserIp)
                .HasMaxLength(100)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.BranchIdfromNavigation).WithMany(p => p.InvTransferMasterBranchIdfromNavigations)
                .HasForeignKey(d => d.BranchIdfrom)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Inv_TransferMaster_BranchMaster1");

            entity.HasOne(d => d.BranchIdtoNavigation).WithMany(p => p.InvTransferMasterBranchIdtoNavigations)
                .HasForeignKey(d => d.BranchIdto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Inv_TransferMaster_BranchMaster");
        });

        modelBuilder.Entity<InvTransitDetail>(entity =>
        {
            entity.HasKey(e => e.TransitDetailId).HasName("PK__Inv_TransitDetail");

            entity.ToTable("Inv_TransitDetail");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(50)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.Batch).WithMany(p => p.InvTransitDetails)
                .HasForeignKey(d => d.BatchId)
                .HasConstraintName("FK_Inv_TransitDetail_Inv_Batch");

            entity.HasOne(d => d.IssuanceDetail).WithMany(p => p.InvTransitDetails)
                .HasForeignKey(d => d.IssuanceDetailId)
                .HasConstraintName("FK_Inv_TransitDetail_Inv_IssuenceDetail");

            entity.HasOne(d => d.ProductDetail).WithMany(p => p.InvTransitDetails)
                .HasForeignKey(d => d.ProductDetailId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Inv_TransitDetail_ProductDetail");

            entity.HasOne(d => d.TransferDetail).WithMany(p => p.InvTransitDetails)
                .HasForeignKey(d => d.TransferDetailId)
                .HasConstraintName("FK_Inv_TransitDetail_Inv_TransferDetail");

            entity.HasOne(d => d.TransitUnitNavigation).WithMany(p => p.InvTransitDetails)
                .HasForeignKey(d => d.TransitUnit)
                .HasConstraintName("FK_Inv_TransitDetail_Inv_SetupUnit");

            entity.HasOne(d => d.Type).WithMany(p => p.InvTransitDetails)
                .HasForeignKey(d => d.TypeId)
                .HasConstraintName("FK_Inv_TransitDetail_Setup_MasterDetail");
        });

        modelBuilder.Entity<InvWastageDetail>(entity =>
        {
            entity.HasKey(e => e.WastageDetailId);

            entity.ToTable("Inv_WastageDetail");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Level1UnitId).HasColumnName("Level1UnitID");
            entity.Property(e => e.Level2UnitId).HasColumnName("Level2UnitID");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(50)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.Batch).WithMany(p => p.InvWastageDetails)
                .HasForeignKey(d => d.BatchId)
                .HasConstraintName("FK_Inv_WastageDetail_Inv_Batch");

            entity.HasOne(d => d.Level1Unit).WithMany(p => p.InvWastageDetailLevel1Units)
                .HasForeignKey(d => d.Level1UnitId)
                .HasConstraintName("FK_Inv_WastageDetail_Inv_SetupUnit");

            entity.HasOne(d => d.Level2Unit).WithMany(p => p.InvWastageDetailLevel2Units)
                .HasForeignKey(d => d.Level2UnitId)
                .HasConstraintName("FK_Inv_WastageDetail_Inv_SetupUnit1");

            entity.HasOne(d => d.ProductDetail).WithMany(p => p.InvWastageDetails)
                .HasForeignKey(d => d.ProductDetailId)
                .HasConstraintName("FK_Inv_WastageDetail_ProductDetail");

            entity.HasOne(d => d.Wastage).WithMany(p => p.InvWastageDetails)
                .HasForeignKey(d => d.WastageId)
                .HasConstraintName("FK_Inv_WastageDetail_Inv_WastageMaster");
        });

        modelBuilder.Entity<InvWastageMaster>(entity =>
        {
            entity.HasKey(e => e.WastageId);

            entity.ToTable("Inv_WastageMaster");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.UserIp)
                .HasMaxLength(50)
                .HasColumnName("UserIP");
            entity.Property(e => e.WastageNumber).HasMaxLength(50);

            entity.HasOne(d => d.Branch).WithMany(p => p.InvWastageMasters)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("FK_Inv_WastageMaster_BranchMaster");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.InvWastageMasterCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_Inv_WastageMaster_UserLogin1");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.InvWastageMasterModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("FK_Inv_WastageMaster_UserLogin2");

            entity.HasOne(d => d.User).WithMany(p => p.InvWastageMasterUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Inv_WastageMaster_UserLogin");
        });

        modelBuilder.Entity<LoyaltyCard>(entity =>
        {
            entity.ToTable("LoyaltyCard");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.LoyaltyCardNumber)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(100)
                .IsFixedLength()
                .HasColumnName("UserIP");

            entity.HasOne(d => d.Company).WithMany(p => p.LoyaltyCards)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("FK_LoyaltyCard_SetupCompany");

            entity.HasOne(d => d.Customer).WithMany(p => p.LoyaltyCards)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_LoyaltyCard_Customer");

            entity.HasOne(d => d.LoyaltyCardType).WithMany(p => p.LoyaltyCards)
                .HasForeignKey(d => d.LoyaltyCardTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LoyaltyCard_LoyaltyCardType");
        });

        modelBuilder.Entity<LoyaltyCardBalance>(entity =>
        {
            entity.ToTable("LoyaltyCardBalance");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .IsUnicode(false)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.Company).WithMany(p => p.LoyaltyCardBalances)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("FK_LoyaltyCardBalance_SetupCompany");

            entity.HasOne(d => d.LoyaltyCard).WithMany(p => p.LoyaltyCardBalances)
                .HasForeignKey(d => d.LoyaltyCardId)
                .HasConstraintName("FK_LoyaltyCardBalance_LoyaltyCard");

            entity.HasOne(d => d.OrderMaster).WithMany(p => p.LoyaltyCardBalances)
                .HasForeignKey(d => d.OrderMasterId)
                .HasConstraintName("FK_LoyaltyCardBalance_OrderMaster");

            entity.HasOne(d => d.Type).WithMany(p => p.LoyaltyCardBalances)
                .HasForeignKey(d => d.TypeId)
                .HasConstraintName("FK_LoyaltyCardBalance_Setup_MasterDetail");
        });

        modelBuilder.Entity<LoyaltyCardType>(entity =>
        {
            entity.ToTable("LoyaltyCardType");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.LoyaltyCardType1)
                .IsUnicode(false)
                .HasColumnName("LoyaltyCardType");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .IsUnicode(false)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.Company).WithMany(p => p.LoyaltyCardTypes)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LoyaltyCardType_SetupCompany");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.ToTable("OrderDetail");

            entity.HasIndex(e => new { e.OrderMasterId, e.IsActive }, "Consumption_index1");

            entity.HasIndex(e => e.IsActive, "NONC_INDEX1");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Gstid).HasColumnName("GSTId");
            entity.Property(e => e.ItemFoc).HasColumnName("ItemFOC");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.PriceWithGst).HasColumnName("PriceWithGST");
            entity.Property(e => e.PriceWithoutGst).HasColumnName("PriceWithoutGST");
            entity.Property(e => e.UserIp).HasColumnName("UserIP");

            entity.HasOne(d => d.CommisionType).WithMany(p => p.OrderDetailCommisionTypes)
                .HasForeignKey(d => d.CommisionTypeId)
                .HasConstraintName("FK_OrderDetail_Setup_MasterDetail");

            entity.HasOne(d => d.DealItem).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.DealItemId)
                .HasConstraintName("FK_OrderDetail_DealItemDetail");

            entity.HasOne(d => d.Discount).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.DiscountId)
                .HasConstraintName("FK_OrderDetail_Discount");

            entity.HasOne(d => d.Gst).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.Gstid)
                .HasConstraintName("FK_OrderDetail_GST");

            entity.HasOne(d => d.OrderMaster).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderMasterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderDetail_OrderMaster");

            entity.HasOne(d => d.ProductDetail).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.ProductDetailId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderDetail_ProductDetail");

            entity.HasOne(d => d.ProductDetailProperty).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.ProductDetailPropertyId)
                .HasConstraintName("FK_OrderDetail_ProductDetailProperty");

            entity.HasOne(d => d.ProductProperty).WithMany(p => p.OrderDetailProductProperties)
                .HasForeignKey(d => d.ProductPropertyId)
                .HasConstraintName("FK_OrderDetail_Setup_MasterDetail1");
        });

        modelBuilder.Entity<OrderDetail20221226>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("OrderDetail_20221226");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Gstid).HasColumnName("GSTId");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.PriceWithGst).HasColumnName("PriceWithGST");
            entity.Property(e => e.PriceWithoutGst).HasColumnName("PriceWithoutGST");
            entity.Property(e => e.UserIp)
                .HasMaxLength(100)
                .HasColumnName("UserIP");
        });

        modelBuilder.Entity<OrderDetail20221228>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("OrderDetail_20221228");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Gstid).HasColumnName("GSTId");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.OrderDetailId).ValueGeneratedOnAdd();
            entity.Property(e => e.PriceWithGst).HasColumnName("PriceWithGST");
            entity.Property(e => e.PriceWithoutGst).HasColumnName("PriceWithoutGST");
            entity.Property(e => e.UserIp)
                .HasMaxLength(100)
                .HasColumnName("UserIP");
        });

        modelBuilder.Entity<OrderDetail20230104>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("OrderDetail_20230104");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Gstid).HasColumnName("GSTId");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.OrderDetailId).ValueGeneratedOnAdd();
            entity.Property(e => e.PriceWithGst).HasColumnName("PriceWithGST");
            entity.Property(e => e.PriceWithoutGst).HasColumnName("PriceWithoutGST");
            entity.Property(e => e.UserIp)
                .HasMaxLength(100)
                .HasColumnName("UserIP");
        });

        modelBuilder.Entity<OrderDetail20230111>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("OrderDetail_20230111");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Gstid).HasColumnName("GSTId");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.OrderDetailId).ValueGeneratedOnAdd();
            entity.Property(e => e.PriceWithGst).HasColumnName("PriceWithGST");
            entity.Property(e => e.PriceWithoutGst).HasColumnName("PriceWithoutGST");
            entity.Property(e => e.UserIp).HasColumnName("UserIP");
        });

        modelBuilder.Entity<OrderDetailLog>(entity =>
        {
            entity.ToTable("OrderDetailLog");

            entity.HasIndex(e => new { e.TypeId, e.IsActive }, "DashBoardIndex1");

            entity.HasIndex(e => new { e.OrderMasterId, e.TypeId, e.IsActive }, "NONC_INDEX1");

            entity.Property(e => e.AmountWithoutGst).HasColumnName("AmountWithoutGST");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ItemFoc).HasColumnName("ItemFOC");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.PriceWithoutGst).HasColumnName("PriceWithoutGST");
            entity.Property(e => e.UserIp).HasColumnName("UserIP");

            entity.HasOne(d => d.OrderMaster).WithMany(p => p.OrderDetailLogs)
                .HasForeignKey(d => d.OrderMasterId)
                .HasConstraintName("FK_OrderDetailLog_OrderMaster");

            entity.HasOne(d => d.ProductDetail).WithMany(p => p.OrderDetailLogs)
                .HasForeignKey(d => d.ProductDetailId)
                .HasConstraintName("FK_OrderDetailLog_ProductDetail");

            entity.HasOne(d => d.Type).WithMany(p => p.OrderDetailLogs)
                .HasForeignKey(d => d.TypeId)
                .HasConstraintName("FK_OrderDetailLog_Setup_MasterDetail");
        });

        modelBuilder.Entity<OrderExtraCharge>(entity =>
        {
            entity.HasKey(e => e.OrderExtraChargesId);

            entity.HasIndex(e => new { e.OrderMasterId, e.IsActive }, "Variance_Report_Index");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp).HasColumnName("UserIP");

            entity.HasOne(d => d.ExtraCharges).WithMany(p => p.OrderExtraCharges)
                .HasForeignKey(d => d.ExtraChargesId)
                .HasConstraintName("FK_OrderExtraCharges_SetupExtraCharges");

            entity.HasOne(d => d.OrderMaster).WithMany(p => p.OrderExtraCharges)
                .HasForeignKey(d => d.OrderMasterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderExtraCharges_OrderMaster");
        });

        modelBuilder.Entity<OrderMaster>(entity =>
        {
            entity.ToTable("OrderMaster");

            entity.HasIndex(e => new { e.IsActive, e.ShiftDetailId }, "DasboardIndex2");

            entity.HasIndex(e => new { e.IsActive, e.TerminalDetailId }, "DashboardIndex");

            entity.HasIndex(e => new { e.CompanyId, e.OrderStatusId, e.IsActive }, "NONC_1");

            entity.HasIndex(e => new { e.OrderStatusId, e.IsActive, e.TerminalDetailId }, "Variance_Report_Index");

            entity.Property(e => e.AdvanceOrderDate).HasColumnType("datetime");
            entity.Property(e => e.AlternateNumber).HasMaxLength(50);
            entity.Property(e => e.Clinumber).HasColumnName("CLINumber");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Gstamount).HasColumnName("GSTAmount");
            entity.Property(e => e.Gstid).HasColumnName("GSTId");
            entity.Property(e => e.Gstpercent).HasColumnName("GSTPercent");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.OrderDate).HasColumnType("datetime");
            entity.Property(e => e.OrderNumber).HasMaxLength(500);
            entity.Property(e => e.OrderSourceValue)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.TotalAmountWithGst).HasColumnName("TotalAmountWithGST");
            entity.Property(e => e.TotalAmountWithoutGst).HasColumnName("TotalAmountWithoutGST");
            entity.Property(e => e.UserIp)
                .HasMaxLength(100)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.Area).WithMany(p => p.OrderMasters)
                .HasForeignKey(d => d.AreaId)
                .HasConstraintName("FK_OrderMaster_Area");

            entity.HasOne(d => d.Branch).WithMany(p => p.OrderMasters)
                .HasForeignKey(d => d.BranchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderMaster_BranchMaster");

            entity.HasOne(d => d.Company).WithMany(p => p.OrderMasters)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderMaster_SetupCompany");

            entity.HasOne(d => d.CustomerAddress).WithMany(p => p.OrderMasters)
                .HasForeignKey(d => d.CustomerAddressId)
                .HasConstraintName("FK_OrderMaster_CustomerAddressDetail");

            entity.HasOne(d => d.Customer).WithMany(p => p.OrderMasters)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_OrderMaster_Customer");

            entity.HasOne(d => d.FinishWasteReason).WithMany(p => p.OrderMasterFinishWasteReasons)
                .HasForeignKey(d => d.FinishWasteReasonId)
                .HasConstraintName("FK_OrderMaster_Setup_MasterDetail3");

            entity.HasOne(d => d.Gst).WithMany(p => p.OrderMasters)
                .HasForeignKey(d => d.Gstid)
                .HasConstraintName("FK_OrderMaster_GST");

            entity.HasOne(d => d.OrderCancelReason).WithMany(p => p.OrderMasterOrderCancelReasons)
                .HasForeignKey(d => d.OrderCancelReasonId)
                .HasConstraintName("FK_OrderMaster_Setup_MasterDetail1");

            entity.HasOne(d => d.OrderMode).WithMany(p => p.OrderMasterOrderModes)
                .HasForeignKey(d => d.OrderModeId)
                .HasConstraintName("FK_OrderMaster_Setup_MasterDetail2");

            entity.HasOne(d => d.OrderSource).WithMany(p => p.OrderMasterOrderSources)
                .HasForeignKey(d => d.OrderSourceId)
                .HasConstraintName("FK_OrderMaster_Setup_MasterDetail");

            entity.HasOne(d => d.Phone).WithMany(p => p.OrderMasters)
                .HasForeignKey(d => d.PhoneId)
                .HasConstraintName("FK_OrderMaster_CustomerPhone");

            entity.HasOne(d => d.Reservation).WithMany(p => p.OrderMasters)
                .HasForeignKey(d => d.ReservationId)
                .HasConstraintName("FK_OrderMaster_ReservationMaster");

            entity.HasOne(d => d.Rider).WithMany(p => p.OrderMasters)
                .HasForeignKey(d => d.RiderId)
                .HasConstraintName("FK_OrderMaster_Rider");

            entity.HasOne(d => d.ShiftDetail).WithMany(p => p.OrderMasters)
                .HasForeignKey(d => d.ShiftDetailId)
                .HasConstraintName("FK_OrderMaster_ShiftDetail");

            entity.HasOne(d => d.Table).WithMany(p => p.OrderMasters)
                .HasForeignKey(d => d.TableId)
                .HasConstraintName("FK_OrderMaster_Table");

            entity.HasOne(d => d.Waiter).WithMany(p => p.OrderMasters)
                .HasForeignKey(d => d.WaiterId)
                .HasConstraintName("FK_OrderMaster_Waiter");
        });

        modelBuilder.Entity<OrderModeOrderSourceMapping>(entity =>
        {
            entity.HasKey(e => e.MappingId);

            entity.ToTable("OrderModeOrderSourceMapping");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Company).WithMany(p => p.OrderModeOrderSourceMappings)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("FK_OrderModeOrderSourceMapping_SetupCompany");

            entity.HasOne(d => d.OrderMode).WithMany(p => p.OrderModeOrderSourceMappingOrderModes)
                .HasForeignKey(d => d.OrderModeId)
                .HasConstraintName("FK_OrderModeOrderSourceMapping_Setup_MasterDetail");

            entity.HasOne(d => d.OrderSource).WithMany(p => p.OrderModeOrderSourceMappingOrderSources)
                .HasForeignKey(d => d.OrderSourceId)
                .HasConstraintName("FK_OrderModeOrderSourceMapping_Setup_MasterDetail1");
        });

        modelBuilder.Entity<OrderPayment>(entity =>
        {
            entity.ToTable("OrderPayment");

            entity.HasIndex(e => e.OrderMasterId, "NONC_INDEX1");

            entity.HasIndex(e => e.IsActive, "NONC_INDEX2");

            entity.HasIndex(e => new { e.PaymentModeId, e.IsActive }, "NONC_INDEX3");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

            entity.HasOne(d => d.OrderMaster).WithMany(p => p.OrderPayments)
                .HasForeignKey(d => d.OrderMasterId)
                .HasConstraintName("FK_OrderPayment_OrderMaster");

            entity.HasOne(d => d.PaymentMode).WithMany(p => p.OrderPayments)
                .HasForeignKey(d => d.PaymentModeId)
                .HasConstraintName("FK_OrderPayment_PaymentMode");

            entity.HasOne(d => d.TerminalDetail).WithMany(p => p.OrderPayments)
                .HasForeignKey(d => d.TerminalDetailId)
                .HasConstraintName("FK_OrderPayment_TerminalDetail");
        });

        modelBuilder.Entity<OrderStatus>(entity =>
        {
            entity.ToTable("OrderStatus");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.OrderStatus1)
                .HasMaxLength(200)
                .HasColumnName("OrderStatus");
            entity.Property(e => e.UserIp)
                .HasMaxLength(100)
                .HasColumnName("UserIP");
        });

        modelBuilder.Entity<OrderStatusLog>(entity =>
        {
            entity.ToTable("OrderStatusLog");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(100)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.Company).WithMany(p => p.OrderStatusLogs)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("FK_OrderStatusLog_SetupCompany");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.OrderStatusLogs)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderStatusLog_UserLogin");

            entity.HasOne(d => d.OrderMaster).WithMany(p => p.OrderStatusLogs)
                .HasForeignKey(d => d.OrderMasterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderStatusLog_OrderMaster");

            entity.HasOne(d => d.OrderStatus).WithMany(p => p.OrderStatusLogs)
                .HasForeignKey(d => d.OrderStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderStatusLog_OrderStatus");
        });

        modelBuilder.Entity<OrderStatusModeMapping>(entity =>
        {
            entity.ToTable("OrderStatusModeMapping");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp).HasColumnName("UserIP");

            entity.HasOne(d => d.OrderMode).WithMany(p => p.OrderStatusModeMappings)
                .HasForeignKey(d => d.OrderModeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderStatusModeMapping_Setup_MasterDetail");

            entity.HasOne(d => d.OrderStatus).WithMany(p => p.OrderStatusModeMappings)
                .HasForeignKey(d => d.OrderStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderStatusModeMapping_OrderStatus");
        });

        modelBuilder.Entity<Orderdetail202212281>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("orderdetail20221228");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Gstid).HasColumnName("GSTId");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.OrderDetailId).ValueGeneratedOnAdd();
            entity.Property(e => e.PriceWithGst).HasColumnName("PriceWithGST");
            entity.Property(e => e.PriceWithoutGst).HasColumnName("PriceWithoutGST");
            entity.Property(e => e.UserIp)
                .HasMaxLength(100)
                .HasColumnName("UserIP");
        });

        modelBuilder.Entity<PayableReceivableVoucherDetail>(entity =>
        {
            entity.ToTable("PayableReceivableVoucherDetail");

            entity.Property(e => e.ChequeNo)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Credit).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Debit).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.InvoiceNo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(50)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.OrderMaster).WithMany(p => p.PayableReceivableVoucherDetails)
                .HasForeignKey(d => d.OrderMasterId)
                .HasConstraintName("FK_PayableReceivableVoucherDetail_OrderMaster");

            entity.HasOne(d => d.PayableReceivableVoucherMaster).WithMany(p => p.PayableReceivableVoucherDetails)
                .HasForeignKey(d => d.PayableReceivableVoucherMasterId)
                .HasConstraintName("FK_PayableReceivableVoucherDetail_PayableReceivableVoucherMaster");
        });

        modelBuilder.Entity<PayableReceivableVoucherMaster>(entity =>
        {
            entity.ToTable("PayableReceivableVoucherMaster");

            entity.Property(e => e.BranchId).HasColumnName("BranchID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(50)
                .HasColumnName("UserIP");
            entity.Property(e => e.VoucherDate).HasColumnType("datetime");

            entity.HasOne(d => d.Customer).WithMany(p => p.PayableReceivableVoucherMasters)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_PayableReceivableVoucherMaster_Customer");

            entity.HasOne(d => d.Vendor).WithMany(p => p.PayableReceivableVoucherMasters)
                .HasForeignKey(d => d.VendorId)
                .HasConstraintName("FK_PayableReceivableVoucherMaster_Vendor");
        });

        modelBuilder.Entity<PaymentMode>(entity =>
        {
            entity.ToTable("PaymentMode");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.PaymentMode1)
                .HasMaxLength(500)
                .HasColumnName("PaymentMode");
            entity.Property(e => e.UserIp)
                .HasMaxLength(50)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.Company).WithMany(p => p.PaymentModes)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("FK_PaymentMode_SetupCompany");
        });

        modelBuilder.Entity<PaymentModeOrderSourceMapping>(entity =>
        {
            entity.HasKey(e => e.MappingId);

            entity.ToTable("PaymentModeOrderSourceMapping");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Company).WithMany(p => p.PaymentModeOrderSourceMappings)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("FK_PaymentModeOrderSourceMapping_SetupCompany");

            entity.HasOne(d => d.OrderSource).WithMany(p => p.PaymentModeOrderSourceMappingOrderSources)
                .HasForeignKey(d => d.OrderSourceId)
                .HasConstraintName("FK_PaymentModeOrderSourceMapping_Setup_MasterDetail1");

            entity.HasOne(d => d.PaymentMode).WithMany(p => p.PaymentModeOrderSourceMappingPaymentModes)
                .HasForeignKey(d => d.PaymentModeId)
                .HasConstraintName("FK_PaymentModeOrderSourceMapping_Setup_MasterDetail");
        });

        modelBuilder.Entity<PaymentVoucherDetail>(entity =>
        {
            entity.ToTable("PaymentVoucherDetail");

            entity.Property(e => e.AccountNo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ChequeNo)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Credit).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Debit).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.InvoiceNo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Rate)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UserIp)
                .HasMaxLength(50)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.ChartOfAccount).WithMany(p => p.PaymentVoucherDetails)
                .HasForeignKey(d => d.ChartOfAccountId)
                .HasConstraintName("FK_PaymentVoucherDetail_ChartOfAccount");

            entity.HasOne(d => d.CostCenter).WithMany(p => p.PaymentVoucherDetails)
                .HasForeignKey(d => d.CostCenterId)
                .HasConstraintName("FK_PaymentVoucherDetail_CostCenter");

            entity.HasOne(d => d.OrderMaster).WithMany(p => p.PaymentVoucherDetails)
                .HasForeignKey(d => d.OrderMasterId)
                .HasConstraintName("FK_PaymentVoucherDetail_OrderMaster");

            entity.HasOne(d => d.PaymentVoucherMaster).WithMany(p => p.PaymentVoucherDetails)
                .HasForeignKey(d => d.PaymentVoucherMasterId)
                .HasConstraintName("FK_PaymentVoucherDetail_PaymentVoucherMaster");

            entity.HasOne(d => d.PymentType).WithMany(p => p.PaymentVoucherDetails)
                .HasForeignKey(d => d.PymentTypeId)
                .HasConstraintName("FK_PaymentVoucherDetail_MasterDetail1");
        });

        modelBuilder.Entity<PaymentVoucherMaster>(entity =>
        {
            entity.ToTable("PaymentVoucherMaster");

            entity.Property(e => e.BranchId).HasColumnName("BranchID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CostCenterId).HasColumnName("CostCenterID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(50)
                .HasColumnName("UserIP");
            entity.Property(e => e.VoucherDate).HasColumnType("datetime");

            entity.HasOne(d => d.Customer).WithMany(p => p.PaymentVoucherMasters)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_PaymentVoucherMaster_Customer");

            entity.HasOne(d => d.VoucherMaster).WithMany(p => p.InverseVoucherMaster)
                .HasForeignKey(d => d.VoucherMasterId)
                .HasConstraintName("FK_PaymentVoucherMaster_PaymentVoucherMaster");

            entity.HasOne(d => d.VoucherType).WithMany(p => p.PaymentVoucherMasters)
                .HasForeignKey(d => d.VoucherTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PaymentVoucherMaster_MasterDetail");
        });

        modelBuilder.Entity<PosAction>(entity =>
        {
            entity.Property(e => e.PosAction1)
                .HasMaxLength(50)
                .HasColumnName("PosAction");
        });

        modelBuilder.Entity<PosRole>(entity =>
        {
            entity.Property(e => e.Role).HasMaxLength(50);
        });

        modelBuilder.Entity<PosRoleActionMapping>(entity =>
        {
            entity.ToTable("PosRoleActionMapping");

            entity.HasOne(d => d.PosAction).WithMany(p => p.PosRoleActionMappings)
                .HasForeignKey(d => d.PosActionId)
                .HasConstraintName("FK_PosRoleActionMapping_PosActions");

            entity.HasOne(d => d.PosRole).WithMany(p => p.PosRoleActionMappings)
                .HasForeignKey(d => d.PosRoleId)
                .HasConstraintName("FK_PosRoleActionMapping_PosRoleActionMapping");
        });

        modelBuilder.Entity<Printer>(entity =>
        {
            entity.ToTable("Printer");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.PrinterIp).HasMaxLength(50);
            entity.Property(e => e.PrinterName).HasMaxLength(50);

            entity.HasOne(d => d.Branch).WithMany(p => p.Printers)
                .HasForeignKey(d => d.BranchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Printer_BranchMaster");
        });

        modelBuilder.Entity<PrinterDepartmentMapping>(entity =>
        {
            entity.HasKey(e => e.PrinterMappingId);

            entity.ToTable("PrinterDepartmentMapping");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp).HasColumnName("UserIP");

            entity.HasOne(d => d.Department).WithMany(p => p.PrinterDepartmentMappings)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("FK_PrinterDepartmentMapping_Department");

            entity.HasOne(d => d.OrderMode).WithMany(p => p.PrinterDepartmentMappings)
                .HasForeignKey(d => d.OrderModeId)
                .HasConstraintName("FK_PrinterDepartmentMapping_Setup_MasterDetail");

            entity.HasOne(d => d.Printer).WithMany(p => p.PrinterDepartmentMappings)
                .HasForeignKey(d => d.PrinterId)
                .HasConstraintName("FK_PrinterDepartmentMapping_Printer");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Product");

            entity.HasIndex(e => new { e.ProductCategoryId, e.IsActive }, "DashboardIndex_1");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DisplayInMobile).HasDefaultValue(true);
            entity.Property(e => e.DisplayInOdms).HasDefaultValue(true);
            entity.Property(e => e.DisplayInPos).HasDefaultValue(true);
            entity.Property(e => e.DisplayInWeb).HasDefaultValue(true);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsEnable).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.ProductName).HasMaxLength(100);
            entity.Property(e => e.UserIp)
                .HasMaxLength(100)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.CommisionType).WithMany(p => p.Products)
                .HasForeignKey(d => d.CommisionTypeId)
                .HasConstraintName("FK_Product_Setup_MasterDetail");

            entity.HasOne(d => d.ProductTag).WithMany(p => p.Products)
                .HasForeignKey(d => d.ProductTagId)
                .HasConstraintName("FK_Product_SetupProductTag");
        });

        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK_Category");

            entity.ToTable("ProductCategory");

            entity.Property(e => e.CategoryBgColor)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CategoryForeColor)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CategoryName).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsEnable).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(100)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.Company).WithMany(p => p.ProductCategories)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("FK_Category_SetupCompany");
        });

        modelBuilder.Entity<ProductDetail>(entity =>
        {
            entity.ToTable("ProductDetail");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsEnable).HasDefaultValue(true);
            entity.Property(e => e.IsSaleable).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Sku).HasColumnName("SKU");
            entity.Property(e => e.UserIp)
                .HasMaxLength(100)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.ConsumeUnit).WithMany(p => p.ProductDetailConsumeUnits)
                .HasForeignKey(d => d.ConsumeUnitId)
                .HasConstraintName("FK_ProductDetail_Inv_SetupUnit2");

            entity.HasOne(d => d.Flavour).WithMany(p => p.ProductDetails)
                .HasForeignKey(d => d.FlavourId)
                .HasConstraintName("FK_ProductDetail_Flavour");

            entity.HasOne(d => d.IssuanceUnit).WithMany(p => p.ProductDetailIssuanceUnits)
                .HasForeignKey(d => d.IssuanceUnitId)
                .HasConstraintName("FK_ProductDetail_Inv_SetupUnit1");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductDetail_Product");

            entity.HasOne(d => d.PurchaseUnit).WithMany(p => p.ProductDetailPurchaseUnits)
                .HasForeignKey(d => d.PurchaseUnitId)
                .HasConstraintName("FK_ProductDetail_Inv_SetupUnit");

            entity.HasOne(d => d.Size).WithMany(p => p.ProductDetails)
                .HasForeignKey(d => d.SizeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductDetail_ProductSize");
        });

        modelBuilder.Entity<ProductDetailAvailability>(entity =>
        {
            entity.HasKey(e => e.ProductDetailAvailableId);

            entity.ToTable("ProductDetailAvailability");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Day).WithMany(p => p.ProductDetailAvailabilities)
                .HasForeignKey(d => d.DayId)
                .HasConstraintName("FK_ProductDetailAvailability_Setup_MasterDetail");

            entity.HasOne(d => d.ProductBranch).WithMany(p => p.ProductDetailAvailabilities)
                .HasForeignKey(d => d.ProductBranchId)
                .HasConstraintName("FK_ProductDetailAvailability_ProductDetailBranchMapping");
        });

        modelBuilder.Entity<ProductDetailBranchMapping>(entity =>
        {
            entity.ToTable("ProductDetailBranchMapping");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsEnable).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(50)
                .HasColumnName("UserIP");
            entity.Property(e => e.ValidFrom).HasColumnType("datetime");
            entity.Property(e => e.ValidTo).HasColumnType("datetime");

            entity.HasOne(d => d.Branch).WithMany(p => p.ProductDetailBranchMappings)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("FK_ProductDetailBranchMapping_BranchMaster");

            entity.HasOne(d => d.ProductDetail).WithMany(p => p.ProductDetailBranchMappings)
                .HasForeignKey(d => d.ProductDetailId)
                .HasConstraintName("FK_ProductDetailBranchMapping_ProductDetail");
        });

        modelBuilder.Entity<ProductDetailCode>(entity =>
        {
            entity.ToTable("ProductDetailCode");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp).HasColumnName("UserIP");

            entity.HasOne(d => d.ProductDetail).WithMany(p => p.ProductDetailCodes)
                .HasForeignKey(d => d.ProductDetailId)
                .HasConstraintName("FK_ProductDetailCode_ProductDetail");
        });

        modelBuilder.Entity<ProductDetailOrderSourcePriceMapping>(entity =>
        {
            entity.HasKey(e => e.MapId).HasName("PK_ProductOrderSourcePriceMapping");

            entity.ToTable("ProductDetailOrderSourcePriceMapping");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

            entity.HasOne(d => d.OrderSource).WithMany(p => p.ProductDetailOrderSourcePriceMappings)
                .HasForeignKey(d => d.OrderSourceId)
                .HasConstraintName("FK__ProductOr__Order__59C61FAD");

            entity.HasOne(d => d.ProductDetail).WithMany(p => p.ProductDetailOrderSourcePriceMappings)
                .HasForeignKey(d => d.ProductDetailId)
                .HasConstraintName("FK__ProductOr__Produ__5ABA43E6");
        });

        modelBuilder.Entity<ProductDetailProperty>(entity =>
        {
            entity.ToTable("ProductDetailProperty");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp).HasColumnName("UserIP");

            entity.HasOne(d => d.ProductDetail).WithMany(p => p.ProductDetailProperties)
                .HasForeignKey(d => d.ProductDetailId)
                .HasConstraintName("FK_ProductDetailProperty_ProductDetail");

            entity.HasOne(d => d.ProductProperty).WithMany(p => p.ProductDetailProperties)
                .HasForeignKey(d => d.ProductPropertyId)
                .HasConstraintName("FK_ProductDetailProperty_Setup_MasterDetail");
        });

        modelBuilder.Entity<ProductDetailToppingMapping>(entity =>
        {
            entity.ToTable("ProductDetailToppingMapping");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.HeaderText).HasMaxLength(50);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(50)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.ProductDetail).WithMany(p => p.ProductDetailToppingMappingProductDetails)
                .HasForeignKey(d => d.ProductDetailId)
                .HasConstraintName("FK_ProductDetailToppingMapping_ProductDetail");

            entity.HasOne(d => d.ProductDetailTopping).WithMany(p => p.ProductDetailToppingMappingProductDetailToppings)
                .HasForeignKey(d => d.ProductDetailToppingId)
                .HasConstraintName("FK_ProductDetailToppingMapping_ProductDetail1");
        });

        modelBuilder.Entity<ProductSize>(entity =>
        {
            entity.HasKey(e => e.SizeId);

            entity.ToTable("ProductSize");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.SizeBgColor)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.SizeForeColor)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.SizeName).HasMaxLength(100);
            entity.Property(e => e.UserIp)
                .HasMaxLength(100)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.Company).WithMany(p => p.ProductSizes)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("FK_ProductSize_SetupCompany");
        });

        modelBuilder.Entity<Province>(entity =>
        {
            entity.ToTable("Province");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.ProvinceName).HasMaxLength(100);
            entity.Property(e => e.UserIp)
                .HasMaxLength(100)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.Country).WithMany(p => p.Provinces)
                .HasForeignKey(d => d.CountryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Province_Country");
        });

        modelBuilder.Entity<ReservationDetail>(entity =>
        {
            entity.ToTable("ReservationDetail");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp).HasColumnName("UserIP");

            entity.HasOne(d => d.ProductDetail).WithMany(p => p.ReservationDetails)
                .HasForeignKey(d => d.ProductDetailId)
                .HasConstraintName("FK_ReservationDetail_ProductDetail");

            entity.HasOne(d => d.Reservation).WithMany(p => p.ReservationDetails)
                .HasForeignKey(d => d.ReservationId)
                .HasConstraintName("FK_ReservationDetail_ReservationMaster");
        });

        modelBuilder.Entity<ReservationMaster>(entity =>
        {
            entity.HasKey(e => e.ReservationId);

            entity.ToTable("ReservationMaster");

            entity.Property(e => e.CheckInTime).HasColumnType("datetime");
            entity.Property(e => e.CheckOutTime).HasColumnType("datetime");
            entity.Property(e => e.Cnic).HasColumnName("CNIC");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CutOffTime).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.ReservationDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp).HasColumnName("UserIP");

            entity.HasOne(d => d.Branch).WithMany(p => p.ReservationMasters)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("FK_ReservationMaster_BranchMaster");

            entity.HasOne(d => d.Company).WithMany(p => p.ReservationMasters)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("FK_ReservationMaster_SetupCompany");

            entity.HasOne(d => d.CustomerAddress).WithMany(p => p.ReservationMasters)
                .HasForeignKey(d => d.CustomerAddressId)
                .HasConstraintName("FK_ReservationMaster_CustomerAddressDetail");

            entity.HasOne(d => d.Customer).WithMany(p => p.ReservationMasters)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_ReservationMaster_Customer");

            entity.HasOne(d => d.GuestType).WithMany(p => p.ReservationMasterGuestTypes)
                .HasForeignKey(d => d.GuestTypeId)
                .HasConstraintName("FK_ReservationMaster_Setup_MasterDetail1");

            entity.HasOne(d => d.PaymentMode).WithMany(p => p.ReservationMasters)
                .HasForeignKey(d => d.PaymentModeId)
                .HasConstraintName("FK_ReservationMaster_PaymentMode");

            entity.HasOne(d => d.Phone).WithMany(p => p.ReservationMasters)
                .HasForeignKey(d => d.PhoneId)
                .HasConstraintName("FK_ReservationMaster_CustomerPhone");

            entity.HasOne(d => d.Slot).WithMany(p => p.ReservationMasterSlots)
                .HasForeignKey(d => d.SlotId)
                .HasConstraintName("FK_ReservationMaster_Setup_MasterDetail");
        });

        modelBuilder.Entity<ReservationStatus>(entity =>
        {
            entity.ToTable("ReservationStatus");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.ReservationStatus1)
                .HasMaxLength(200)
                .HasColumnName("ReservationStatus");
            entity.Property(e => e.UserIp)
                .HasMaxLength(100)
                .HasColumnName("UserIP");
        });

        modelBuilder.Entity<Rider>(entity =>
        {
            entity.ToTable("Rider");

            entity.Property(e => e.Cnic)
                .HasMaxLength(50)
                .HasColumnName("CNIC");
            entity.Property(e => e.Contact1).HasMaxLength(50);
            entity.Property(e => e.Contact2).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.RiderName).HasMaxLength(50);
            entity.Property(e => e.UserIp)
                .HasMaxLength(50)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.Branch).WithMany(p => p.Riders)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("FK_Rider_BranchMaster");
        });

        modelBuilder.Entity<SalesReturnDetail>(entity =>
        {
            entity.HasKey(e => e.SalesReturnDetailId).HasName("PK__SalesRet__B6BCC94873906956");

            entity.ToTable("SalesReturnDetail");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(50)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.Batch).WithMany(p => p.SalesReturnDetails)
                .HasForeignKey(d => d.BatchId)
                .HasConstraintName("FK_SalesReturnDetail_Inv_Batch");

            entity.HasOne(d => d.OrderDetail).WithMany(p => p.SalesReturnDetails)
                .HasForeignKey(d => d.OrderDetailId)
                .HasConstraintName("FK_SalesReturnDetail_OrderDetail");

            entity.HasOne(d => d.ProductDetail).WithMany(p => p.SalesReturnDetails)
                .HasForeignKey(d => d.ProductDetailId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SalesRetu__Produ__5812E165");

            entity.HasOne(d => d.SalesReturn).WithMany(p => p.SalesReturnDetails)
                .HasForeignKey(d => d.SalesReturnId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SalesRetu__Sales__5847EB8F");
        });

        modelBuilder.Entity<SalesReturnMaster>(entity =>
        {
            entity.HasKey(e => e.SalesReturnId).HasName("PK__SalesRet__E0906C382A275500");

            entity.ToTable("SalesReturnMaster");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.SalesReturnNumber).HasMaxLength(50);
            entity.Property(e => e.UserIp)
                .HasMaxLength(50)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.Branch).WithMany(p => p.SalesReturnMasters)
                .HasForeignKey(d => d.BranchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SalesReturnMaster_BranchMaster");

            entity.HasOne(d => d.Company).WithMany(p => p.SalesReturnMasters)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("FK_SalesReturnMaster_SetupCompany");

            entity.HasOne(d => d.OrderMaster).WithMany(p => p.SalesReturnMasters)
                .HasForeignKey(d => d.OrderMasterId)
                .HasConstraintName("FK__SalesRetu__Order__556B7EE4");
        });

        modelBuilder.Entity<SetupBank>(entity =>
        {
            entity.HasKey(e => e.BankId).HasName("PK_Bank");

            entity.ToTable("Setup_Bank");

            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("UserIP");
        });

        modelBuilder.Entity<SetupBankDetail>(entity =>
        {
            entity.HasKey(e => e.BankDetailId).HasName("PK_BankDetail");

            entity.ToTable("Setup_Bank_Detail");

            entity.Property(e => e.AccountNo).HasMaxLength(50);
            entity.Property(e => e.BankId).HasColumnName("BankID");
            entity.Property(e => e.BranchId).HasColumnName("BranchID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("UserIP");
        });

        modelBuilder.Entity<SetupCompany>(entity =>
        {
            entity.HasKey(e => e.CompanyId).HasName("PK_Company");

            entity.ToTable("SetupCompany");

            entity.Property(e => e.CompanyCode).HasMaxLength(50);
            entity.Property(e => e.CompanyName).HasMaxLength(50);
            entity.Property(e => e.CountryId).HasDefaultValue(2);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsEnable).HasDefaultValue(true);
            entity.Property(e => e.IsPos).HasDefaultValue(true);
            entity.Property(e => e.IsSrbintegration).HasColumnName("IsSRBIntegration");
            entity.Property(e => e.IsValidCompany).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.BusinessType).WithMany(p => p.SetupCompanyBusinessTypes)
                .HasForeignKey(d => d.BusinessTypeId)
                .HasConstraintName("FK_SetupCompany_Setup_MasterDetail");

            entity.HasOne(d => d.Country).WithMany(p => p.SetupCompanies)
                .HasForeignKey(d => d.CountryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SetupCompany_Setup_MasterDetail2");

            entity.HasOne(d => d.Currency).WithMany(p => p.SetupCompanyCurrencies)
                .HasForeignKey(d => d.CurrencyId)
                .HasConstraintName("FK_SetupCompany_Setup_MasterDetail1");
        });

        modelBuilder.Entity<SetupCompanySetting>(entity =>
        {
            entity.HasKey(e => e.SettingId);

            entity.ToTable("SetupCompanySetting");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp).HasColumnName("UserIP");

            entity.HasOne(d => d.Branch).WithMany(p => p.SetupCompanySettings)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("FK_SetupCompanySetting_BranchMaster");

            entity.HasOne(d => d.Company).WithMany(p => p.SetupCompanySettings)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("FK_SetupCompanySetting_SetupCompany");

            entity.HasOne(d => d.SetupDetail).WithMany(p => p.SetupCompanySettings)
                .HasForeignKey(d => d.SetupDetailId)
                .HasConstraintName("FK_SetupCompanySetting_Setup_MasterDetail");
        });

        modelBuilder.Entity<SetupExtraCharge>(entity =>
        {
            entity.HasKey(e => e.ExtraChargesId);

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp).HasColumnName("UserIP");

            entity.HasOne(d => d.Company).WithMany(p => p.SetupExtraCharges)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SetupExtraCharges_SetupCompany");

            entity.HasOne(d => d.OrderMode).WithMany(p => p.SetupExtraCharges)
                .HasForeignKey(d => d.OrderModeId)
                .HasConstraintName("FK_SetupExtraCharges_Setup_MasterDetail");
        });

        modelBuilder.Entity<SetupFeature>(entity =>
        {
            entity.HasKey(e => e.FeatureId);

            entity.ToTable("Setup_Feature");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Feature).HasMaxLength(50);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<SetupMaster>(entity =>
        {
            entity.HasKey(e => e.SetupMasterId).HasName("PK_SetupType");

            entity.ToTable("Setup_Master");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.SetupMasterName).HasMaxLength(50);
        });

        modelBuilder.Entity<SetupMasterDetail>(entity =>
        {
            entity.HasKey(e => e.SetupDetailId).HasName("PK_SetupTypeDetail");

            entity.ToTable("Setup_MasterDetail");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(200)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.Company).WithMany(p => p.SetupMasterDetails)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("FK_Setup_MasterDetail_SetupCompany");

            entity.HasOne(d => d.SetupMaster).WithMany(p => p.SetupMasterDetails)
                .HasForeignKey(d => d.SetupMasterId)
                .HasConstraintName("FK_SetupTypeDetail_SetupTypeMaster");
        });

        modelBuilder.Entity<SetupMenuItem>(entity =>
        {
            entity.HasKey(e => e.MenuId).HasName("PK__MenuItem__956B9FF52D27B809");

            entity.ToTable("Setup_MenuItem");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsDisplayedInMenu)
                .HasDefaultValue(true)
                .HasColumnName("Is_Displayed_In_Menu");
            entity.Property(e => e.MenuName)
                .HasMaxLength(100)
                .HasColumnName("Menu_Name");
            entity.Property(e => e.MenuUrl)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Menu_URL");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.ParentId).HasColumnName("Parent_Id");
            entity.Property(e => e.UserIp)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK_Setup_MenuItem_Setup_MenuItem");
        });

        modelBuilder.Entity<SetupMenuItemFeatureMapping>(entity =>
        {
            entity.HasKey(e => e.MenuItemFeatureId).HasName("PK_Sec_Setup_MenuItemFeature");

            entity.ToTable("Setup_MenuItemFeatureMapping");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.Feature).WithMany(p => p.SetupMenuItemFeatureMappings)
                .HasForeignKey(d => d.FeatureId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Setup_MenuItemFeatureMapping_Setup_Feature");

            entity.HasOne(d => d.Menu).WithMany(p => p.SetupMenuItemFeatureMappings)
                .HasForeignKey(d => d.MenuId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Setup_MenuItemFeature_Setup_MenuItem");
        });

        modelBuilder.Entity<SetupProductTag>(entity =>
        {
            entity.HasKey(e => e.ProductTagId);

            entity.ToTable("SetupProductTag");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(50)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.Company).WithMany(p => p.SetupProductTags)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("FK_SetupProductTag_SetupCompany");
        });

        modelBuilder.Entity<SetupRoleAccess>(entity =>
        {
            entity.HasKey(e => e.RoleAccessCode).HasName("PK__RoleAcce__70AF398C276EDEB3");

            entity.ToTable("Setup_RoleAccess");

            entity.Property(e => e.RoleAccessCode).HasColumnName("Role_Access_Code");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.HasAccess).HasColumnName("Has_Access");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.Company).WithMany(p => p.SetupRoleAccesses)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("FK_Setup_RoleAccess_SetupCompany");
        });

        modelBuilder.Entity<SetupRoleAccessAction>(entity =>
        {
            entity.ToTable("SetupRoleAccessAction");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsAccess).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(50)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.Company).WithMany(p => p.SetupRoleAccessActions)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("FK_SetupRoleAccessAction_SetupCompany");

            entity.HasOne(d => d.Role).WithMany(p => p.SetupRoleAccessActions)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_SetupRoleAccessAction_UserRole");

            entity.HasOne(d => d.SetupDetail).WithMany(p => p.SetupRoleAccessActions)
                .HasForeignKey(d => d.SetupDetailId)
                .HasConstraintName("FK_SetupRoleAccessAction_Setup_MasterDetail");
        });

        modelBuilder.Entity<SetupRoleMenuItemFeatureMapping>(entity =>
        {
            entity.HasKey(e => e.RoleMenuItemFeatureMappingId);

            entity.ToTable("Setup_RoleMenuItemFeatureMapping");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.RoleAccessCode).HasColumnName("Role_Access_Code");
            entity.Property(e => e.UserIp)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.SetupRoleMenuItemFeatureMappingCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_Setup_RoleMenuItemFeatureMapping_UserLogin");

            entity.HasOne(d => d.MenuItemFeature).WithMany(p => p.SetupRoleMenuItemFeatureMappings)
                .HasForeignKey(d => d.MenuItemFeatureId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Setup_RoleMenuItemFeatureMapping_Setup_MenuItemFeature");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.SetupRoleMenuItemFeatureMappingModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("FK_Setup_RoleMenuItemFeatureMapping_UserLogin1");

            entity.HasOne(d => d.RoleAccessCodeNavigation).WithMany(p => p.SetupRoleMenuItemFeatureMappings)
                .HasForeignKey(d => d.RoleAccessCode)
                .HasConstraintName("FK_Setup_RoleMenuItemFeatureMapping_Setup_RoleAccess");
        });

        modelBuilder.Entity<SetupTypeDetail>(entity =>
        {
            entity.HasKey(e => e.TypeDetailId);

            entity.ToTable("Setup_TypeDetail");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Type).WithMany(p => p.SetupTypeDetails)
                .HasForeignKey(d => d.TypeId)
                .HasConstraintName("FK_Setup_TypeDetail_SetupTypeMaster");
        });

        modelBuilder.Entity<SetupTypeMaster>(entity =>
        {
            entity.HasKey(e => e.TypeId);

            entity.ToTable("SetupTypeMaster");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.TypeName).HasMaxLength(50);
        });

        modelBuilder.Entity<Shift>(entity =>
        {
            entity.ToTable("Shift");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Prefix).HasMaxLength(50);
            entity.Property(e => e.ShiftName).HasMaxLength(50);
            entity.Property(e => e.UserIp).HasColumnName("UserIP");

            entity.HasOne(d => d.Company).WithMany(p => p.Shifts)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("FK_Shift_SetupCompany");
        });

        modelBuilder.Entity<ShiftDetail>(entity =>
        {
            entity.ToTable("ShiftDetail");

            entity.Property(e => e.ClosingDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.OpeningDate).HasColumnType("datetime");
            entity.Property(e => e.ShiftNum).HasMaxLength(50);

            entity.HasOne(d => d.Branch).WithMany(p => p.ShiftDetails)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("FK_ShiftDetail_BranchMaster");

            entity.HasOne(d => d.BusinessDay).WithMany(p => p.ShiftDetails)
                .HasForeignKey(d => d.BusinessDayId)
                .HasConstraintName("FK_ShiftDetail_BusinessDay");

            entity.HasOne(d => d.Shift).WithMany(p => p.ShiftDetails)
                .HasForeignKey(d => d.ShiftId)
                .HasConstraintName("FK_ShiftDetail_Shift");
        });

        modelBuilder.Entity<Table>(entity =>
        {
            entity.HasKey(e => e.TableId).HasName("PK_SetupTable");

            entity.ToTable("Table");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsOpen).HasDefaultValue(false);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.TableName).HasMaxLength(50);
            entity.Property(e => e.UserIp)
                .HasMaxLength(50)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.Branch).WithMany(p => p.Tables)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("FK_Table_BranchMaster");
        });

        modelBuilder.Entity<TableMerge>(entity =>
        {
            entity.ToTable("TableMerge");

            entity.Property(e => e.AllowUnmerge).HasDefaultValue(true);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Branch).WithMany(p => p.TableMerges)
                .HasForeignKey(d => d.BranchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TableMerge_BranchMaster");

            entity.HasOne(d => d.OrderMaster).WithMany(p => p.TableMerges)
                .HasForeignKey(d => d.OrderMasterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TableMerge_OrderMaster");

            entity.HasOne(d => d.Table).WithMany(p => p.TableMerges)
                .HasForeignKey(d => d.TableId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TableMerge_Table");
        });

        modelBuilder.Entity<TableMergeDetail>(entity =>
        {
            entity.ToTable("TableMergeDetail");

            entity.Property(e => e.TableMergeDetailId).ValueGeneratedNever();
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

            entity.HasOne(d => d.OrderMaster).WithMany(p => p.TableMergeDetails)
                .HasForeignKey(d => d.OrderMasterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TableMergeDetail_OrderMaster");

            entity.HasOne(d => d.Table).WithMany(p => p.TableMergeDetails)
                .HasForeignKey(d => d.TableId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TableMergeDetail_Table");

            entity.HasOne(d => d.TableMergeMaster).WithMany(p => p.TableMergeDetails)
                .HasForeignKey(d => d.TableMergeMasterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TableMergeDetail_TableMerge");
        });

        modelBuilder.Entity<TblFiscalMonth>(entity =>
        {
            entity.HasKey(e => e.FiscalMonthId);

            entity.ToTable("TBL_Fiscal_Month");

            entity.Property(e => e.ClosingBalance).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TblFiscalYear>(entity =>
        {
            entity.HasKey(e => e.YearId);

            entity.ToTable("TBL_Fiscal_Year");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.FiscalYearName).HasMaxLength(250);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Isclosed).HasColumnName("isclosed");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.YearFrom).HasColumnType("datetime");
            entity.Property(e => e.YearTo).HasColumnType("datetime");
        });

        modelBuilder.Entity<TblGetorderList>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("TBL_GETOrderList");

            entity.Property(e => e.AdvanceOrderDate).HasColumnType("datetime");
            entity.Property(e => e.AlternateNumber).HasMaxLength(50);
            entity.Property(e => e.BranchName).HasMaxLength(200);
            entity.Property(e => e.Clinumber).HasColumnName("CLINumber");
            entity.Property(e => e.DeliveryTime).HasMaxLength(35);
            entity.Property(e => e.Gstamount).HasColumnName("GSTAmount");
            entity.Property(e => e.Gstid).HasColumnName("GSTId");
            entity.Property(e => e.Gstpercent).HasColumnName("GSTPercent");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.OrderDate).HasMaxLength(30);
            entity.Property(e => e.OrderDateTime)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.OrderDeliveryDateTime)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.OrderNumber).HasMaxLength(500);
            entity.Property(e => e.OrderSourceValue)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.OrderStatus).HasMaxLength(200);
            entity.Property(e => e.PhoneNumber).HasMaxLength(50);
            entity.Property(e => e.RiderName).HasMaxLength(50);
            entity.Property(e => e.TotalAmountWithGst).HasColumnName("TotalAmountWithGST");
            entity.Property(e => e.TotalAmountWithoutGst).HasColumnName("TotalAmountWithoutGST");
            entity.Property(e => e.UserIp)
                .HasMaxLength(100)
                .HasColumnName("UserIP");
        });

        modelBuilder.Entity<TblPayOff>(entity =>
        {
            entity.HasKey(e => e.PayOffId);

            entity.ToTable("tbl_PayOFF");

            entity.Property(e => e.PayOffId).HasColumnName("PayOffID");
            entity.Property(e => e.VoucherMasterId).HasColumnName("VoucherMasterID");
        });

        modelBuilder.Entity<TblPocCostCenter>(entity =>
        {
            entity.HasKey(e => e.CostCenterPocId);

            entity.ToTable("Tbl_POC_Cost_Center");

            entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");
            entity.Property(e => e.Ip).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");
            entity.Property(e => e.PoccontactNo)
                .HasMaxLength(13)
                .HasColumnName("POCContactNo");
            entity.Property(e => e.Pocemail)
                .HasMaxLength(100)
                .HasColumnName("POCEmail");
            entity.Property(e => e.Pocname)
                .HasMaxLength(200)
                .HasColumnName("POCName");

            entity.HasOne(d => d.CostCenter).WithMany(p => p.TblPocCostCenters)
                .HasForeignKey(d => d.CostCenterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_POC_Cost_Center_CostCenter");
        });

        modelBuilder.Entity<TempOrderDetail>(entity =>
        {
            entity.ToTable("TempOrderDetail");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp).HasColumnName("UserIP");

            entity.HasOne(d => d.DealItem).WithMany(p => p.TempOrderDetails)
                .HasForeignKey(d => d.DealItemId)
                .HasConstraintName("FK_TempOrderDetail_DealItemDetail");

            entity.HasOne(d => d.ProductDetail).WithMany(p => p.TempOrderDetails)
                .HasForeignKey(d => d.ProductDetailId)
                .HasConstraintName("FK_TempOrderDetail_ProductDetail");

            entity.HasOne(d => d.TempOrderMaster).WithMany(p => p.TempOrderDetails)
                .HasForeignKey(d => d.TempOrderMasterId)
                .HasConstraintName("FK_TempOrderDetail_TempOrderMaster");

            entity.HasOne(d => d.TempOrderParent).WithMany(p => p.InverseTempOrderParent)
                .HasForeignKey(d => d.TempOrderParentId)
                .HasConstraintName("FK_TempOrderDetail_TempOrderDetail");
        });

        modelBuilder.Entity<TempOrderMaster>(entity =>
        {
            entity.ToTable("TempOrderMaster");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.OrderDate).HasColumnType("datetime");
            entity.Property(e => e.OrderNumber).HasMaxLength(50);
            entity.Property(e => e.UserIp)
                .HasMaxLength(50)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.Branch).WithMany(p => p.TempOrderMasters)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("FK_TempOrderMaster_BranchMaster");

            entity.HasOne(d => d.BusinessDay).WithMany(p => p.TempOrderMasters)
                .HasForeignKey(d => d.BusinessDayId)
                .HasConstraintName("FK_TempOrderMaster_BusinessDay");

            entity.HasOne(d => d.Company).WithMany(p => p.TempOrderMasters)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("FK_TempOrderMaster_SetupCompany");

            entity.HasOne(d => d.ShiftDetail).WithMany(p => p.TempOrderMasters)
                .HasForeignKey(d => d.ShiftDetailId)
                .HasConstraintName("FK_TempOrderMaster_ShiftDetail");

            entity.HasOne(d => d.TerminalDetail).WithMany(p => p.TempOrderMasters)
                .HasForeignKey(d => d.TerminalDetailId)
                .HasConstraintName("FK_TempOrderMaster_TerminalDetail");
        });

        modelBuilder.Entity<Template>(entity =>
        {
            entity.ToTable("Template");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsEnable).HasDefaultValue(true);
            entity.Property(e => e.IsSelected).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.TemplateName).HasMaxLength(50);
            entity.Property(e => e.UserIp).HasColumnName("UserIP");

            entity.HasOne(d => d.Company).WithMany(p => p.Templates)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Template_SetupCompany");

            entity.HasOne(d => d.TemplateType).WithMany(p => p.Templates)
                .HasForeignKey(d => d.TemplateTypeId)
                .HasConstraintName("FK_Template_Setup_MasterDetail");
        });

        modelBuilder.Entity<Terminal>(entity =>
        {
            entity.HasKey(e => e.TerminalId).HasName("PK_Counter");

            entity.ToTable("Terminal");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.TerminalName).HasMaxLength(50);
            entity.Property(e => e.UniqueId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.UserIp).HasColumnName("UserIP");

            entity.HasOne(d => d.Branch).WithMany(p => p.Terminals)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("FK_Terminal_BranchMaster");

            entity.HasOne(d => d.Company).WithMany(p => p.Terminals)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("FK_Counter_SetupCompany");
        });

        modelBuilder.Entity<TerminalDetail>(entity =>
        {
            entity.HasKey(e => e.TerminalDetailId).HasName("PK_CounterDetail");

            entity.ToTable("TerminalDetail");

            entity.HasIndex(e => new { e.BusinessDayId, e.IsActive }, "DashboardIndex");

            entity.HasIndex(e => new { e.ShiftDetailId, e.IsActive }, "DashboardIndex2");

            entity.Property(e => e.ClosingDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.OpeningDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(50)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.Branch).WithMany(p => p.TerminalDetails)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("FK_CounterDetail_BranchMaster");

            entity.HasOne(d => d.BusinessDay).WithMany(p => p.TerminalDetails)
                .HasForeignKey(d => d.BusinessDayId)
                .HasConstraintName("FK_CounterDetail_BusinessDay");

            entity.HasOne(d => d.ShiftDetail).WithMany(p => p.TerminalDetails)
                .HasForeignKey(d => d.ShiftDetailId)
                .HasConstraintName("FK_CounterDetail_ShiftDetail");

            entity.HasOne(d => d.Terminal).WithMany(p => p.TerminalDetails)
                .HasForeignKey(d => d.TerminalId)
                .HasConstraintName("FK_CounterDetail_Counter");
        });

        modelBuilder.Entity<UserBranchMapping>(entity =>
        {
            entity.HasKey(e => e.UserBranchId);

            entity.ToTable("UserBranchMapping");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.UserIp)
                .IsUnicode(false)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.Branch).WithMany(p => p.UserBranchMappings)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("FK_UserBranchMapping_BranchMaster");

            entity.HasOne(d => d.User).WithMany(p => p.UserBranchMappings)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_UserBranchMapping_UserLogin");
        });

        modelBuilder.Entity<UserLogin>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.ToTable("UserLogin");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsEnable).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp).HasColumnName("UserIP");

            entity.HasOne(d => d.PosRole).WithMany(p => p.UserLogins)
                .HasForeignKey(d => d.PosRoleId)
                .HasConstraintName("FK_UserLogin_PosRoles");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK_Roles");

            entity.ToTable("UserRole");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.RoleName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.UserIp)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("UserIP");

            entity.HasOne(d => d.Company).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("FK_UserRole_SetupCompany");
        });

        modelBuilder.Entity<VendorProductDetailMapping>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("VendorProductDetailMapping");

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.VpdmId).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<VwGrn>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("VW_GRN");

            entity.Property(e => e.CompanyName).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ExpiryDate).HasColumnType("datetime");
            entity.Property(e => e.Isexpirymandatory).HasColumnName("isexpirymandatory");
            entity.Property(e => e.ProductName).HasMaxLength(100);
            entity.Property(e => e.Qty).HasColumnName("QTY");
        });

        modelBuilder.Entity<Waiter>(entity =>
        {
            entity.ToTable("Waiter");

            entity.Property(e => e.Cnic)
                .HasMaxLength(50)
                .HasColumnName("CNIC");
            entity.Property(e => e.Contact1).HasMaxLength(50);
            entity.Property(e => e.Contact2).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserIp)
                .HasMaxLength(50)
                .HasColumnName("UserIP");
            entity.Property(e => e.WaiterName).HasMaxLength(50);

            entity.HasOne(d => d.Branch).WithMany(p => p.Waiters)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("FK_Waiter_BranchMaster");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
