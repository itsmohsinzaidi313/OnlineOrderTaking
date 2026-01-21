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

    public virtual DbSet<Area> Areas { get; set; }

    public virtual DbSet<BranchDayMapping> BranchDayMappings { get; set; }

    public virtual DbSet<BranchDetail> BranchDetails { get; set; }

    public virtual DbSet<BranchMaster> BranchMasters { get; set; }

    public virtual DbSet<CategoryAvailability> CategoryAvailabilities { get; set; }

    public virtual DbSet<City> Cities { get; set; }

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

    public virtual DbSet<SetupCompanySetting> SetupCompanySettings { get; set; }

    public virtual DbSet<SetupMaster> SetupMasters { get; set; }

    public virtual DbSet<SetupMasterDetail> SetupMasterDetails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Area>(entity =>
        {
            entity.ToTable("area");

            entity.Property(e => e.AreaName).HasMaxLength(200);
        });

        modelBuilder.Entity<BranchDayMapping>(entity =>
        {
            entity.ToTable("branch_day_mapping");

            entity.HasIndex(e => e.DayId, "IX_branch_day_mapping_DayId");

            entity.HasOne(d => d.Day).WithMany(p => p.BranchDayMappings).HasForeignKey(d => d.DayId);
        });

        modelBuilder.Entity<BranchDetail>(entity =>
        {
            entity.ToTable("branch_detail");

            entity.HasIndex(e => e.AreaId, "IX_branch_detail_AreaId");

            entity.HasIndex(e => e.BranchId, "IX_branch_detail_BranchId");

            entity.Property(e => e.AreaName).HasMaxLength(150);

            entity.HasOne(d => d.Area).WithMany(p => p.BranchDetails).HasForeignKey(d => d.AreaId);

            entity.HasOne(d => d.Branch).WithMany(p => p.BranchDetails).HasForeignKey(d => d.BranchId);
        });

        modelBuilder.Entity<BranchMaster>(entity =>
        {
            entity.HasKey(e => e.BranchId);

            entity.ToTable("branch_master");

            entity.Property(e => e.BranchAddress).HasMaxLength(300);
            entity.Property(e => e.BranchName).HasMaxLength(200);
            entity.Property(e => e.BranchPhoneNumber).HasMaxLength(100);
            entity.Property(e => e.CityName).HasMaxLength(150);
            entity.Property(e => e.Ntnname)
                .HasMaxLength(150)
                .HasColumnName("NTNName");
            entity.Property(e => e.Ntnnumber)
                .HasMaxLength(100)
                .HasColumnName("NTNNumber");
        });

        modelBuilder.Entity<CategoryAvailability>(entity =>
        {
            entity.HasKey(e => e.CategoryAvailableId);

            entity.ToTable("category_availability");

            entity.HasIndex(e => e.CategoryId, "IX_category_availability_CategoryId");

            entity.HasIndex(e => e.DayId, "IX_category_availability_DayId");

            entity.HasOne(d => d.Category).WithMany(p => p.CategoryAvailabilities).HasForeignKey(d => d.CategoryId);

            entity.HasOne(d => d.Day).WithMany(p => p.CategoryAvailabilities).HasForeignKey(d => d.DayId);
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.ToTable("city");

            entity.Property(e => e.CityName).HasMaxLength(150);
        });

        modelBuilder.Entity<DealDescription>(entity =>
        {
            entity.HasKey(e => e.DealDescId);

            entity.ToTable("deal_description");

            entity.HasIndex(e => e.DealItemId, "IX_deal_description_DealItemId");

            entity.HasIndex(e => e.ProductDetailId, "IX_deal_description_ProductDetailId");

            entity.HasOne(d => d.DealItem).WithMany(p => p.DealDescriptions).HasForeignKey(d => d.DealItemId);

            entity.HasOne(d => d.ProductDetail).WithMany(p => p.DealDescriptions).HasForeignKey(d => d.ProductDetailId);
        });

        modelBuilder.Entity<DealItemDetail>(entity =>
        {
            entity.HasKey(e => e.DealItemId);

            entity.ToTable("deal_item_detail");
        });

        modelBuilder.Entity<Discount>(entity =>
        {
            entity.ToTable("discount");

            entity.Property(e => e.DiscountCapEnd).HasPrecision(18, 2);
            entity.Property(e => e.DiscountCapStart).HasPrecision(18, 2);
            entity.Property(e => e.DiscountName).HasMaxLength(255);
            entity.Property(e => e.EndDate).HasColumnType("timestamp without time zone");
            entity.Property(e => e.IsActiveInOdms).HasColumnName("IsActiveInODMS");
            entity.Property(e => e.IsActiveInPos).HasColumnName("IsActiveInPOS");
            entity.Property(e => e.StartDate).HasColumnType("timestamp without time zone");
            entity.Property(e => e.VocherCodeEnd).HasMaxLength(100);
            entity.Property(e => e.VocherCodeStart).HasMaxLength(100);
        });

        modelBuilder.Entity<DiscountBranchMapping>(entity =>
        {
            entity.ToTable("discount_branch_mapping");

            entity.HasIndex(e => e.BranchId, "IX_discount_branch_mapping_BranchId");

            entity.HasIndex(e => e.DiscountId, "IX_discount_branch_mapping_DiscountId");

            entity.HasOne(d => d.Branch).WithMany(p => p.DiscountBranchMappings).HasForeignKey(d => d.BranchId);

            entity.HasOne(d => d.Discount).WithMany(p => p.DiscountBranchMappings).HasForeignKey(d => d.DiscountId);
        });

        modelBuilder.Entity<DiscountDayMapping>(entity =>
        {
            entity.ToTable("discount_day_mapping");

            entity.HasIndex(e => e.DiscountId, "IX_discount_day_mapping_DiscountId");

            entity.HasOne(d => d.Discount).WithMany(p => p.DiscountDayMappings).HasForeignKey(d => d.DiscountId);
        });

        modelBuilder.Entity<DiscountOrderModeMapping>(entity =>
        {
            entity.ToTable("discount_order_mode_mapping");

            entity.HasIndex(e => e.DiscountId, "IX_discount_order_mode_mapping_DiscountId");

            entity.HasOne(d => d.Discount).WithMany(p => p.DiscountOrderModeMappings).HasForeignKey(d => d.DiscountId);
        });

        modelBuilder.Entity<DiscountOrderTypeMapping>(entity =>
        {
            entity.ToTable("discount_order_type_mapping");

            entity.HasIndex(e => e.DiscountId, "IX_discount_order_type_mapping_DiscountId");

            entity.HasOne(d => d.Discount).WithMany(p => p.DiscountOrderTypeMappings).HasForeignKey(d => d.DiscountId);
        });

        modelBuilder.Entity<DiscountProductDetailMapping>(entity =>
        {
            entity.ToTable("discount_product_detail_mapping");

            entity.HasIndex(e => e.DiscountId, "IX_discount_product_detail_mapping_DiscountId");

            entity.HasIndex(e => e.ProductDetailId, "IX_discount_product_detail_mapping_ProductDetailId");

            entity.HasOne(d => d.Discount).WithMany(p => p.DiscountProductDetailMappings).HasForeignKey(d => d.DiscountId);

            entity.HasOne(d => d.ProductDetail).WithMany(p => p.DiscountProductDetailMappings)
                .HasForeignKey(d => d.ProductDetailId)
                .HasConstraintName("FK_discount_product_detail_mapping_product_detail_ProductDetai~");
        });

        modelBuilder.Entity<Flavour>(entity =>
        {
            entity.ToTable("flavour");

            entity.Property(e => e.FlavourName).HasMaxLength(150);
        });

        modelBuilder.Entity<Gst>(entity =>
        {
            entity.ToTable("gst");

            entity.HasIndex(e => e.CityId, "IX_gst_CityId");

            entity.HasIndex(e => e.CompanyId, "IX_gst_CompanyId");

            entity.HasIndex(e => e.PaymentModeId, "IX_gst_PaymentModeId");

            entity.Property(e => e.Gstid).HasColumnName("GSTId");
            entity.Property(e => e.Gstname)
                .HasMaxLength(100)
                .HasColumnName("GSTName");
            entity.Property(e => e.Gstpercentage).HasColumnName("GSTPercentage");

            entity.HasOne(d => d.City).WithMany(p => p.Gsts).HasForeignKey(d => d.CityId);

            entity.HasOne(d => d.Company).WithMany(p => p.Gsts).HasForeignKey(d => d.CompanyId);

            entity.HasOne(d => d.PaymentMode).WithMany(p => p.Gsts).HasForeignKey(d => d.PaymentModeId);
        });

        modelBuilder.Entity<OrderModeCompanyMapping>(entity =>
        {
            entity.HasKey(e => e.OrderModeMappingId);

            entity.ToTable("order_mode_company_mapping");

            entity.HasIndex(e => e.CompanyId, "IX_order_mode_company_mapping_CompanyId");

            entity.HasOne(d => d.Company).WithMany(p => p.OrderModeCompanyMappings).HasForeignKey(d => d.CompanyId);
        });

        modelBuilder.Entity<PaymentMode>(entity =>
        {
            entity.ToTable("payment_mode");

            entity.HasIndex(e => e.CompanyId, "IX_payment_mode_CompanyId");

            entity.Property(e => e.PaymentMode1)
                .HasMaxLength(150)
                .HasColumnName("PaymentMode");

            entity.HasOne(d => d.Company).WithMany(p => p.PaymentModes).HasForeignKey(d => d.CompanyId);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("product");

            entity.HasIndex(e => e.ProductCategoryId, "IX_product_ProductCategoryId");

            entity.Property(e => e.ProductImage).HasMaxLength(250);
            entity.Property(e => e.ProductName).HasMaxLength(200);

            entity.HasOne(d => d.ProductCategory).WithMany(p => p.Products).HasForeignKey(d => d.ProductCategoryId);
        });

        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId);

            entity.ToTable("product_category");

            entity.HasIndex(e => e.CompanyId, "IX_product_category_CompanyId");

            entity.Property(e => e.CategoryBgColor).HasMaxLength(50);
            entity.Property(e => e.CategoryForeColor).HasMaxLength(50);
            entity.Property(e => e.CategoryIcon).HasMaxLength(100);
            entity.Property(e => e.CategoryImage).HasMaxLength(300);
            entity.Property(e => e.CategoryName).HasMaxLength(200);
            entity.Property(e => e.ProductCardStyle).HasMaxLength(100);

            entity.HasOne(d => d.Company).WithMany(p => p.ProductCategories).HasForeignKey(d => d.CompanyId);
        });

        modelBuilder.Entity<ProductDetail>(entity =>
        {
            entity.ToTable("product_detail");

            entity.HasIndex(e => e.FlavourId, "IX_product_detail_FlavourId");

            entity.HasIndex(e => e.ProductId, "IX_product_detail_ProductId");

            entity.HasIndex(e => e.SizeId, "IX_product_detail_SizeId");

            entity.HasOne(d => d.Flavour).WithMany(p => p.ProductDetails).HasForeignKey(d => d.FlavourId);

            entity.HasOne(d => d.Product).WithMany(p => p.ProductDetails).HasForeignKey(d => d.ProductId);

            entity.HasOne(d => d.Size).WithMany(p => p.ProductDetails).HasForeignKey(d => d.SizeId);
        });

        modelBuilder.Entity<ProductDetailAvailability>(entity =>
        {
            entity.HasKey(e => e.ProductDetailAvailableId);

            entity.ToTable("product_detail_availability");

            entity.HasIndex(e => e.DayId, "IX_product_detail_availability_DayId");

            entity.HasIndex(e => e.ProductBranchId, "IX_product_detail_availability_ProductBranchId");

            entity.HasOne(d => d.Day).WithMany(p => p.ProductDetailAvailabilities).HasForeignKey(d => d.DayId);

            entity.HasOne(d => d.ProductBranch).WithMany(p => p.ProductDetailAvailabilities)
                .HasForeignKey(d => d.ProductBranchId)
                .HasConstraintName("FK_product_detail_availability_product_detail_branch_mapping_P~");
        });

        modelBuilder.Entity<ProductDetailBranchMapping>(entity =>
        {
            entity.ToTable("product_detail_branch_mapping");

            entity.HasIndex(e => e.BranchId, "IX_product_detail_branch_mapping_BranchId");

            entity.HasIndex(e => e.ProductDetailId, "IX_product_detail_branch_mapping_ProductDetailId");

            entity.Property(e => e.RemoteId).HasMaxLength(100);

            entity.HasOne(d => d.Branch).WithMany(p => p.ProductDetailBranchMappings).HasForeignKey(d => d.BranchId);

            entity.HasOne(d => d.ProductDetail).WithMany(p => p.ProductDetailBranchMappings).HasForeignKey(d => d.ProductDetailId);
        });

        modelBuilder.Entity<ProductDetailOrderSourcePriceMapping>(entity =>
        {
            entity.HasKey(e => e.MapId);

            entity.ToTable("product_detail_order_source_price_mapping");

            entity.HasIndex(e => e.BranchId, "IX_product_detail_order_source_price_mapping_BranchId");

            entity.HasIndex(e => e.OrderSourceId, "IX_product_detail_order_source_price_mapping_OrderSourceId");

            entity.HasIndex(e => e.ProductDetailId, "IX_product_detail_order_source_price_mapping_ProductDetailId");

            entity.HasOne(d => d.Branch).WithMany(p => p.ProductDetailOrderSourcePriceMappings)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("FK_product_detail_order_source_price_mapping_branch_master_Bra~");

            entity.HasOne(d => d.OrderSource).WithMany(p => p.ProductDetailOrderSourcePriceMappings)
                .HasForeignKey(d => d.OrderSourceId)
                .HasConstraintName("FK_product_detail_order_source_price_mapping_setup_master_deta~");

            entity.HasOne(d => d.ProductDetail).WithMany(p => p.ProductDetailOrderSourcePriceMappings)
                .HasForeignKey(d => d.ProductDetailId)
                .HasConstraintName("FK_product_detail_order_source_price_mapping_product_detail_Pr~");
        });

        modelBuilder.Entity<ProductSize>(entity =>
        {
            entity.HasKey(e => e.SizeId);

            entity.ToTable("product_size");

            entity.Property(e => e.SizeName).HasMaxLength(100);
        });

        modelBuilder.Entity<SetupCompany>(entity =>
        {
            entity.HasKey(e => e.CompanyId);

            entity.ToTable("setup_company");
        });

        modelBuilder.Entity<SetupCompanySetting>(entity =>
        {
            entity.HasKey(e => e.SettingId);

            entity.ToTable("setup_company_setting");
        });

        modelBuilder.Entity<SetupMaster>(entity =>
        {
            entity.ToTable("setup_master");
        });

        modelBuilder.Entity<SetupMasterDetail>(entity =>
        {
            entity.HasKey(e => e.SetupDetailId);

            entity.ToTable("setup_master_detail");

            entity.Property(e => e.ConstantValue).HasColumnName("Constant_Value");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
