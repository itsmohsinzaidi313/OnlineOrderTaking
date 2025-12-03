using Microsoft.EntityFrameworkCore;

namespace PointofSaleModels.PGDatabaseModels;

public partial class PgDbContext : DbContext
{
    public PgDbContext()
    {
    }

    public PgDbContext(DbContextOptions<PgDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<OrderNumberSequence> OrderNumberSequences { get; set; }

    public virtual DbSet<Area> Areas { get; set; }

    public virtual DbSet<BranchDayMapping> BranchDayMappings { get; set; }

    public virtual DbSet<BranchDetail> BranchDetails { get; set; }

    public virtual DbSet<BranchMaster> BranchMasters { get; set; }

    public virtual DbSet<CategoryAvailability> CategoryAvailabilities { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<CustomerAddressDetail> CustomerAddressDetails { get; set; }

    public virtual DbSet<CustomerPhone> CustomerPhones { get; set; }

    public virtual DbSet<DealDescription> DealDescriptions { get; set; }

    public virtual DbSet<DealItemDetail> DealItemDetails { get; set; }

    public virtual DbSet<Discount> Discounts { get; set; }

    public virtual DbSet<DiscountBranchMapping> DiscountBranchMappings { get; set; }

    public virtual DbSet<DiscountDayMapping> DiscountDayMappings { get; set; }

    public virtual DbSet<DiscountOrderModeMapping> DiscountOrderModeMappings { get; set; }

    public virtual DbSet<DiscountOrderTypeMapping> DiscountOrderTypeMappings { get; set; }

    public virtual DbSet<DiscountProductDetailMapping> DiscountProductDetailMappings { get; set; }

    public virtual DbSet<Flavour> Flavours { get; set; }

    public virtual DbSet<Gst> Gsts { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<OrderMaster> OrderMasters { get; set; }

    public virtual DbSet<OrderModeCompanyMapping> OrderModeCompanyMappings { get; set; }

    public virtual DbSet<PaymentMode> PaymentModes { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductCategory> ProductCategories { get; set; }

    public virtual DbSet<ProductDetail> ProductDetails { get; set; }

    public virtual DbSet<ProductDetailAvailability> ProductDetailAvailabilities { get; set; }

    public virtual DbSet<ProductDetailBranchMapping> ProductDetailBranchMappings { get; set; }

    public virtual DbSet<ProductDetailOrderSourcePriceMapping> ProductDetailOrderSourcePriceMappings { get; set; }

    public virtual DbSet<ProductSize> ProductSizes { get; set; }

    public virtual DbSet<SetupCompany> SetupCompanies { get; set; }

    public virtual DbSet<SetupMaster> SetupMasters { get; set; }

    public virtual DbSet<SetupMasterDetail> SetupMasterDetails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Area>(entity =>
        {
            entity.HasKey(e => e.AreaId).HasName("area_pkey");

            entity.ToTable("area");

            entity.Property(e => e.AreaName).HasMaxLength(200);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsEnable).HasDefaultValue(true);
        });

        modelBuilder.Entity<BranchDayMapping>(entity =>
        {
            entity.HasKey(e => e.BranchDayMappingId).HasName("branch_day_mapping_pkey");

            entity.ToTable("branch_day_mapping");

            entity.Property(e => e.DayName).HasMaxLength(100);
            entity.Property(e => e.DayNumber).HasMaxLength(10);
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Day).WithMany(p => p.BranchDayMappings)
                .HasForeignKey(d => d.DayId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_branch_day_mapping_day");
        });

        modelBuilder.Entity<BranchDetail>(entity =>
        {
            entity.HasKey(e => e.BranchDetailId).HasName("branch_detail_pkey");

            entity.ToTable("branch_detail");

            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Branch).WithMany(p => p.BranchDetails)
                .HasForeignKey(d => d.BranchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_branch_detail_branch");
        });

        modelBuilder.Entity<BranchMaster>(entity =>
        {
            entity.HasKey(e => e.BranchId).HasName("branch_master_pkey");

            entity.ToTable("branch_master");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsCallCenter).HasDefaultValue(false);
            entity.Property(e => e.Ntnnumber).HasColumnName("NTNNumber");

            entity.HasOne(d => d.Company).WithMany(p => p.BranchMasters)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_branch_company");
        });

        modelBuilder.Entity<CategoryAvailability>(entity =>
        {
            entity.HasKey(e => e.CategoryAvailableId).HasName("category_availability_pkey");

            entity.ToTable("category_availability");

            entity.Property(e => e.CategoryAvailableId).ValueGeneratedNever();
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Category).WithMany(p => p.CategoryAvailabilities)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_categoryavailability_category");

            entity.HasOne(d => d.Day).WithMany(p => p.CategoryAvailabilities)
                .HasForeignKey(d => d.DayId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_categoryavailability_day");
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.CityId).HasName("city_pkey");

            entity.ToTable("city");

            entity.Property(e => e.CityName).HasMaxLength(150);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("customer_pkey");

            entity.ToTable("customer");

            entity.Property(e => e.Cnic).HasColumnName("CNIC");
            entity.Property(e => e.Gst)
                .HasMaxLength(50)
                .HasColumnName("GST");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsPrimary).HasDefaultValue(false);
            entity.Property(e => e.Ntn)
                .HasMaxLength(50)
                .HasColumnName("NTN");
            entity.Property(e => e.Sst)
                .HasMaxLength(50)
                .HasColumnName("SST");
            entity.Property(e => e.Title).HasMaxLength(50);

            entity.HasOne(d => d.Company).WithMany(p => p.Customers)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("fk_customer_setup_company");

            entity.HasOne(d => d.Phone).WithMany(p => p.Customers)
                .HasForeignKey(d => d.PhoneId)
                .HasConstraintName("fk_customer_customer_phone");
        });

        modelBuilder.Entity<CustomerAddressDetail>(entity =>
        {
            entity.HasKey(e => e.CustomerAddressId).HasName("customer_address_detail_pkey");

            entity.ToTable("customer_address_detail");

            entity.Property(e => e.BlockFloor).HasMaxLength(50);
            entity.Property(e => e.Building).HasMaxLength(200);
            entity.Property(e => e.CompanyName).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsPrimary).HasDefaultValue(false);
            entity.Property(e => e.LandMark).HasMaxLength(200);
            entity.Property(e => e.RoomHouse).HasMaxLength(50);
            entity.Property(e => e.StreetRowLane).HasMaxLength(100);

            entity.HasOne(d => d.Area).WithMany(p => p.CustomerAddressDetails)
                .HasForeignKey(d => d.AreaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_customer_address_detail_area");

            entity.HasOne(d => d.City).WithMany(p => p.CustomerAddressDetails)
                .HasForeignKey(d => d.CityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_customer_address_detail_city");

            entity.HasOne(d => d.Company).WithMany(p => p.CustomerAddressDetails)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_customer_address_detail_setup_company");

            entity.HasOne(d => d.Phone).WithMany(p => p.CustomerAddressDetails)
                .HasForeignKey(d => d.PhoneId)
                .HasConstraintName("fk_customer_address_detail_customer_phone");
        });

        modelBuilder.Entity<CustomerPhone>(entity =>
        {
            entity.HasKey(e => e.PhoneId).HasName("customer_phone_pkey");

            entity.ToTable("customer_phone");

            entity.Property(e => e.PhoneNumber).HasMaxLength(50);

            entity.HasOne(d => d.Company).WithMany(p => p.CustomerPhones)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("fk_customer_phone_setup_company");
        });

        modelBuilder.Entity<DealDescription>(entity =>
        {
            entity.HasKey(e => e.DealDescId).HasName("deal_description_pkey");

            entity.ToTable("deal_description");

            entity.Property(e => e.DealDescId).ValueGeneratedNever();
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<DealItemDetail>(entity =>
        {
            entity.HasKey(e => e.DealItemId).HasName("deal_item_detail_pkey");

            entity.ToTable("deal_item_detail");

            entity.Property(e => e.DealItemId).ValueGeneratedNever();
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.ProductDetail).WithMany(p => p.DealItemDetails)
                .HasForeignKey(d => d.ProductDetailId)
                .HasConstraintName("fk_dealitemdetail_productdetail");
        });

        modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasKey(e => e.DiscountId).HasName("discount_pkey");

            entity.ToTable("discount");

            entity.Property(e => e.DiscountId).ValueGeneratedNever();
            entity.Property(e => e.DiscountCapEnd).HasPrecision(18, 2);
            entity.Property(e => e.DiscountCapStart).HasPrecision(18, 2);
            entity.Property(e => e.IsActiveInOdms).HasColumnName("IsActiveInODMS");
            entity.Property(e => e.IsActiveInPos).HasColumnName("IsActiveInPOS");
        });

        modelBuilder.Entity<DiscountBranchMapping>(entity =>
        {
            entity.HasKey(e => e.DiscountBranchMappingId).HasName("discount_branch_mapping_pkey");

            entity.ToTable("discount_branch_mapping");

            entity.Property(e => e.DiscountBranchMappingId).ValueGeneratedNever();

            entity.HasOne(d => d.Discount).WithMany(p => p.DiscountBranchMappings)
                .HasForeignKey(d => d.DiscountId)
                .HasConstraintName("fk_discountbranchmapping_discount");
        });

        modelBuilder.Entity<DiscountDayMapping>(entity =>
        {
            entity.HasKey(e => e.DiscountDayMappingId).HasName("discount_day_mapping_pkey");

            entity.ToTable("discount_day_mapping");

            entity.Property(e => e.DiscountDayMappingId).ValueGeneratedNever();

            entity.HasOne(d => d.Discount).WithMany(p => p.DiscountDayMappings)
                .HasForeignKey(d => d.DiscountId)
                .HasConstraintName("fk_discountdaymapping_discount");
        });

        modelBuilder.Entity<DiscountOrderModeMapping>(entity =>
        {
            entity.HasKey(e => e.DiscountOrderModeMappingId).HasName("discount_order_mode_mapping_pkey");

            entity.ToTable("discount_order_mode_mapping");

            entity.Property(e => e.DiscountOrderModeMappingId).ValueGeneratedNever();

            entity.HasOne(d => d.Discount).WithMany(p => p.DiscountOrderModeMappings)
                .HasForeignKey(d => d.DiscountId)
                .HasConstraintName("fk_discountordermode_discount");
        });

        modelBuilder.Entity<DiscountOrderTypeMapping>(entity =>
        {
            entity.HasKey(e => e.DiscountOrderTypeMappingId).HasName("discount_order_type_mapping_pkey");

            entity.ToTable("discount_order_type_mapping");

            entity.Property(e => e.DiscountOrderTypeMappingId).ValueGeneratedNever();

            entity.HasOne(d => d.Discount).WithMany(p => p.DiscountOrderTypeMappings)
                .HasForeignKey(d => d.DiscountId)
                .HasConstraintName("fk_discountordertype_discount");
        });

        modelBuilder.Entity<DiscountProductDetailMapping>(entity =>
        {
            entity.HasKey(e => e.DiscountProductDetailMappingId).HasName("discount_product_detail_mapping_pkey");

            entity.ToTable("discount_product_detail_mapping");

            entity.Property(e => e.DiscountProductDetailMappingId).ValueGeneratedNever();

            entity.HasOne(d => d.Discount).WithMany(p => p.DiscountProductDetailMappings)
                .HasForeignKey(d => d.DiscountId)
                .HasConstraintName("fk_discountproductdetail_discount");

            entity.HasOne(d => d.ProductDetail).WithMany(p => p.DiscountProductDetailMappings)
                .HasForeignKey(d => d.ProductDetailId)
                .HasConstraintName("fk_discountproductdetail_productdetail");
        });

        modelBuilder.Entity<Flavour>(entity =>
        {
            entity.HasKey(e => e.FlavourId).HasName("flavour_pkey");

            entity.ToTable("flavour");

            entity.Property(e => e.FlavourName).HasMaxLength(150);
        });

        modelBuilder.Entity<Gst>(entity =>
        {
            entity.HasKey(e => e.Gstid).HasName("gst_pkey");

            entity.ToTable("gst");

            entity.Property(e => e.Gstid).HasColumnName("GSTId");
            entity.Property(e => e.Gstname)
                .HasMaxLength(100)
                .HasColumnName("GSTName");
            entity.Property(e => e.Gstpercentage).HasColumnName("GSTPercentage");

            entity.HasOne(d => d.City).WithMany(p => p.Gsts)
                .HasForeignKey(d => d.CityId)
                .HasConstraintName("fk_gst_city");

            entity.HasOne(d => d.Company).WithMany(p => p.Gsts)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("fk_gst_company");

            entity.HasOne(d => d.PaymentMode).WithMany(p => p.Gsts)
                .HasForeignKey(d => d.PaymentModeId)
                .HasConstraintName("fk_gst_payment_mode");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderDetailId).HasName("order_detail_pkey");

            entity.ToTable("order_detail");

            entity.Property(e => e.Gstid).HasColumnName("GSTId");
            entity.Property(e => e.IsKot).HasDefaultValue(false);
            entity.Property(e => e.IsTopping).HasDefaultValue(false);
            entity.Property(e => e.PriceWithGst).HasColumnName("PriceWithGST");
            entity.Property(e => e.PriceWithoutGst).HasColumnName("PriceWithoutGST");

            entity.HasOne(d => d.DealItem).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.DealItemId)
                .HasConstraintName("fk_order_detail_deal_item_detail");

            entity.HasOne(d => d.Discount).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.DiscountId)
                .HasConstraintName("fk_order_detail_discount");

            entity.HasOne(d => d.Gst).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.Gstid)
                .HasConstraintName("fk_order_detail_gst");

            entity.HasOne(d => d.OrderMaster).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderMasterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_order_detail_order_master");

            entity.HasOne(d => d.ProductDetail).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.ProductDetailId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_order_detail_product_detail");
        });

        modelBuilder.Entity<OrderMaster>(entity =>
        {
            entity.HasKey(e => e.OrderMasterId).HasName("order_master_pkey");

            entity.ToTable("order_master");

            entity.Property(e => e.AlternateNumber).HasMaxLength(50);
            entity.Property(e => e.BankName).HasMaxLength(50);
            entity.Property(e => e.CardNumber).HasMaxLength(50);
            entity.Property(e => e.Clinumber).HasColumnName("CLINumber");
            entity.Property(e => e.Gstamount).HasColumnName("GSTAmount");
            entity.Property(e => e.Gstid).HasColumnName("GSTId");
            entity.Property(e => e.Gstpercent).HasColumnName("GSTPercent");
            entity.Property(e => e.IsSyncToPos).HasDefaultValue(false);
            entity.Property(e => e.OrderNumber).HasMaxLength(500);
            entity.Property(e => e.OrderSourceValue).HasMaxLength(250);
            entity.Property(e => e.OrderTime).HasPrecision(6);
            entity.Property(e => e.ReceivedAmount).HasColumnName("Received_Amount");
            entity.Property(e => e.TotalAmountWithGst).HasColumnName("TotalAmountWithGST");
            entity.Property(e => e.TotalAmountWithoutGst).HasColumnName("TotalAmountWithoutGST");
            entity.Property(e => e.VoucherCode).HasMaxLength(50);

            entity.HasOne(d => d.Area).WithMany(p => p.OrderMasters)
                .HasForeignKey(d => d.AreaId)
                .HasConstraintName("fk_order_master_area");

            entity.HasOne(d => d.Branch).WithMany(p => p.OrderMasters)
                .HasForeignKey(d => d.BranchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_order_master_branch_master");

            entity.HasOne(d => d.Company).WithMany(p => p.OrderMasters)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_order_master_setup_company");

            entity.HasOne(d => d.Gst).WithMany(p => p.OrderMasters)
                .HasForeignKey(d => d.Gstid)
                .HasConstraintName("fk_order_master_gst");

            entity.HasOne(d => d.PreviousOrderMaster).WithMany(p => p.InversePreviousOrderMaster)
                .HasForeignKey(d => d.PreviousOrderMasterId)
                .HasConstraintName("fk_order_master_previous_order");
        });

        modelBuilder.Entity<OrderModeCompanyMapping>(entity =>
        {
            entity.HasKey(e => e.OrderModeMappingId).HasName("order_mode_company_mapping_pkey");

            entity.ToTable("order_mode_company_mapping");

            entity.Property(e => e.OrderModeMappingId).ValueGeneratedNever();

            entity.HasOne(d => d.Company).WithMany(p => p.OrderModeCompanyMappings)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_ordermodecompanymapping_company");
        });

        modelBuilder.Entity<PaymentMode>(entity =>
        {
            entity.HasKey(e => e.PaymentModeId).HasName("payment_mode_pkey");

            entity.ToTable("payment_mode");

            entity.Property(e => e.PaymentMode1).HasColumnName("PaymentMode");

            entity.HasOne(d => d.Company).WithMany(p => p.PaymentModes)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("fk_payment_mode_company");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("product_pkey");

            entity.ToTable("product");

            entity.Property(e => e.ProductImage).HasMaxLength(250);
            entity.Property(e => e.ProductName).HasMaxLength(200);

            entity.HasOne(d => d.ProductCategory).WithMany(p => p.Products)
                .HasForeignKey(d => d.ProductCategoryId)
                .HasConstraintName("fk_product_category");
        });

        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("product_category_pkey");

            entity.ToTable("product_category");

            entity.Property(e => e.CategoryBgColor).HasMaxLength(50);
            entity.Property(e => e.CategoryForeColor).HasMaxLength(50);
            entity.Property(e => e.CategoryIcon).HasMaxLength(150);
            entity.Property(e => e.CategoryImage).HasMaxLength(250);
            entity.Property(e => e.CategoryName).HasMaxLength(150);
            entity.Property(e => e.ProductCardStyle).HasMaxLength(150);

            entity.HasOne(d => d.Company).WithMany(p => p.ProductCategories)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("fk_product_category_company");
        });

        modelBuilder.Entity<ProductDetail>(entity =>
        {
            entity.HasKey(e => e.ProductDetailId).HasName("product_detail_pkey");

            entity.ToTable("product_detail");

            entity.Property(e => e.ProductDetailId).ValueGeneratedNever();
            entity.Property(e => e.FlavourName).HasMaxLength(150);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsBestSeller).HasDefaultValue(false);
            entity.Property(e => e.IsDealDirectPunch).HasDefaultValue(false);
            entity.Property(e => e.IsEnable).HasDefaultValue(true);
            entity.Property(e => e.IsOpen).HasDefaultValue(false);
            entity.Property(e => e.IsPromotion).HasDefaultValue(false);
            entity.Property(e => e.IsSaleable).HasDefaultValue(true);
            entity.Property(e => e.IsTopping).HasDefaultValue(false);
            entity.Property(e => e.OnlyForDeal).HasDefaultValue(false);
            entity.Property(e => e.RemoteId).HasMaxLength(100);
            entity.Property(e => e.SizeName).HasMaxLength(150);

            entity.HasOne(d => d.ParentProductDetail).WithMany(p => p.InverseParentProductDetail)
                .HasForeignKey(d => d.ParentProductDetailId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_productdetail_parent");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductDetails)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("fk_productdetail_product");
        });

        modelBuilder.Entity<ProductDetailAvailability>(entity =>
        {
            entity.HasKey(e => e.ProductDetailAvailableId).HasName("product_detail_availability_pkey");

            entity.ToTable("product_detail_availability");

            entity.Property(e => e.ProductDetailAvailableId).ValueGeneratedNever();

            entity.HasOne(d => d.Day).WithMany(p => p.ProductDetailAvailabilities)
                .HasForeignKey(d => d.DayId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_productdetailavailability_day");

            entity.HasOne(d => d.ProductBranch).WithMany(p => p.ProductDetailAvailabilities)
                .HasForeignKey(d => d.ProductBranchId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_productdetailavailability_branch");
        });

        modelBuilder.Entity<ProductDetailBranchMapping>(entity =>
        {
            entity.HasKey(e => e.ProductDetailBranchMappingId).HasName("product_detail_branch_mapping_pkey");

            entity.ToTable("product_detail_branch_mapping");

            entity.Property(e => e.ProductDetailBranchMappingId).ValueGeneratedNever();
            entity.Property(e => e.IsDayWise).HasDefaultValue(false);
            entity.Property(e => e.IsEnable).HasDefaultValue(false);
            entity.Property(e => e.RemoteId).HasMaxLength(100);

            entity.HasOne(d => d.Branch).WithMany(p => p.ProductDetailBranchMappings)
                .HasForeignKey(d => d.BranchId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_productdetailbranchmapping_branch");

            entity.HasOne(d => d.ProductDetail).WithMany(p => p.ProductDetailBranchMappings)
                .HasForeignKey(d => d.ProductDetailId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_productdetailbranchmapping_product");
        });

        modelBuilder.Entity<ProductDetailOrderSourcePriceMapping>(entity =>
        {
            entity.HasKey(e => e.MapId).HasName("product_detail_order_source_price_mapping_pkey");

            entity.ToTable("product_detail_order_source_price_mapping");

            entity.Property(e => e.MapId).ValueGeneratedNever();
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Branch).WithMany(p => p.ProductDetailOrderSourcePriceMappings)
                .HasForeignKey(d => d.BranchId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_productdetailordersourceprice_branch");

            entity.HasOne(d => d.OrderSource).WithMany(p => p.ProductDetailOrderSourcePriceMappings)
                .HasForeignKey(d => d.OrderSourceId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_productdetailordersourceprice_ordersource");

            entity.HasOne(d => d.ProductDetail).WithMany(p => p.ProductDetailOrderSourcePriceMappings)
                .HasForeignKey(d => d.ProductDetailId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_productdetailordersourceprice_productdetail");
        });

        modelBuilder.Entity<ProductSize>(entity =>
        {
            entity.HasKey(e => e.SizeId).HasName("product_size_pkey");

            entity.ToTable("product_size");

            entity.Property(e => e.SizeName).HasMaxLength(100);
        });

        modelBuilder.Entity<SetupCompany>(entity =>
        {
            entity.HasKey(e => e.CompanyId).HasName("setup_company_pkey");

            entity.ToTable("setup_company");

            entity.Property(e => e.ApiUrl).HasMaxLength(255);
            entity.Property(e => e.CompanyLogo).HasMaxLength(255);
            entity.Property(e => e.CompanyName).HasMaxLength(200);
            entity.Property(e => e.Contact1).HasMaxLength(50);
            entity.Property(e => e.Contact2).HasMaxLength(50);
            entity.Property(e => e.EmailAddress).HasMaxLength(150);
            entity.Property(e => e.WebsiteUrl).HasMaxLength(255);
        });

        modelBuilder.Entity<SetupMaster>(entity =>
        {
            entity.HasKey(e => e.SetupMasterId).HasName("setup_master_pkey");

            entity.ToTable("setup_master");

            entity.Property(e => e.SetupMasterName).HasMaxLength(200);
        });

        modelBuilder.Entity<SetupMasterDetail>(entity =>
        {
            entity.HasKey(e => e.SetupDetailId).HasName("setup_master_detail_pkey");

            entity.ToTable("setup_master_detail");

            entity.Property(e => e.ConstantValue).HasColumnName("Constant_Value");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
