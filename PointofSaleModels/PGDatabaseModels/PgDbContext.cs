using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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

    public virtual DbSet<BranchOrderSequence> BranchOrderSequences { get; set; }

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

    public virtual DbSet<SetupCompanySetting> SetupCompanySettings { get; set; }

    public virtual DbSet<SetupMaster> SetupMasters { get; set; }

    public virtual DbSet<SetupMasterDetail> SetupMasterDetails { get; set; }

    public virtual DbSet<BranchOrderSequence> OrderNumberSequences { get; set; }

    public virtual DbSet<UserLogin> UserLogins { get; set; }

    public virtual DbSet<UserBranchMapping> UserBranchMappings { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<OrderStatus> OrderStatuses { get; set; }
    public virtual DbSet<OrderStatusLog> OrderStatusLogs { get; set; }
    public virtual DbSet<Rider> Riders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Area>(entity =>
        {
            entity.ToTable("area");
        });

        modelBuilder.Entity<BranchDayMapping>(entity =>
        {
            entity.ToTable("branch_day_mapping");
        });

        modelBuilder.Entity<BranchDetail>(entity =>
        {
            entity.ToTable("branch_detail");
        });

        modelBuilder.Entity<BranchMaster>(entity =>
        {
            entity.HasKey(e => e.BranchId);

            entity.ToTable("branch_master");

            entity.Property(e => e.Ntnname).HasColumnName("NTNName");
            entity.Property(e => e.Ntnnumber).HasColumnName("NTNNumber");
        });

        modelBuilder.Entity<BranchOrderSequence>(entity =>
        {
            entity.HasKey(e => e.BranchId);

            entity.ToTable("branch_order_sequence");
        });

        modelBuilder.Entity<CategoryAvailability>(entity =>
        {
            entity.HasKey(e => e.CategoryAvailableId);

            entity.ToTable("category_availability");
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.ToTable("city");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("customer");
        });

        modelBuilder.Entity<CustomerAddressDetail>(entity =>
        {
            entity.HasKey(e => e.CustomerAddressId);

            entity.ToTable("customer_address_detail");
        });

        modelBuilder.Entity<CustomerPhone>(entity =>
        {
            entity.ToTable("customer_phone")
                .HasKey(x => x.PhoneId);
            entity
                .HasMany(x => x.Customers)
                .WithOne(x => x.CustomerPhone)
                .HasForeignKey(x => x.PhoneId)
                .HasPrincipalKey(x => x.PhoneId);

            entity
                .HasMany(x => x.CustomerAddressDetails)
                .WithOne(x => x.CustomerPhone)
                .HasForeignKey(x => x.PhoneId)
                .HasPrincipalKey(x => x.PhoneId);
        });

        modelBuilder.Entity<DealDescription>(entity =>
        {
            entity.HasKey(e => e.DealDescId);

            entity.ToTable("deal_description");
        });

        modelBuilder.Entity<DealItemDetail>(entity =>
        {
            entity.HasKey(e => e.DealItemId);

            entity.ToTable("deal_item_detail");
        });

        modelBuilder.Entity<Discount>(entity =>
        {
            entity.ToTable("discount");

            entity.Property(e => e.EndDate).HasColumnType("timestamp without time zone");
            entity.Property(e => e.IsActiveInOdms).HasColumnName("IsActiveInODMS");
            entity.Property(e => e.IsActiveInPos).HasColumnName("IsActiveInPOS");
            entity.Property(e => e.StartDate).HasColumnType("timestamp without time zone");
        });

        modelBuilder.Entity<DiscountBranchMapping>(entity =>
        {
            entity.ToTable("discount_branch_mapping");
        });

        modelBuilder.Entity<DiscountDayMapping>(entity =>
        {
            entity.ToTable("discount_day_mapping");
        });

        modelBuilder.Entity<DiscountOrderModeMapping>(entity =>
        {
            entity.ToTable("discount_order_mode_mapping");
        });

        modelBuilder.Entity<DiscountOrderTypeMapping>(entity =>
        {
            entity.ToTable("discount_order_type_mapping");
        });

        modelBuilder.Entity<DiscountProductDetailMapping>(entity =>
        {
            entity.ToTable("discount_product_detail_mapping");
        });

        modelBuilder.Entity<Flavour>(entity =>
        {
            entity.ToTable("flavour");
        });

        modelBuilder.Entity<Gst>(entity =>
        {
            entity.ToTable("gst");

            entity.Property(e => e.Gstid).HasColumnName("GSTId");
            entity.Property(e => e.Gstname).HasColumnName("GSTName");
            entity.Property(e => e.Gstpercentage).HasColumnName("GSTPercentage");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.ToTable("order_detail");
        });

        modelBuilder.Entity<OrderMaster>(entity =>
        {
            entity.ToTable("order_master");
            var dateOnlyConverter = new DateOnlyToDateTimeConverter();
            entity
                .Property(o => o.OrderDate)
                .HasColumnType("timestamp without time zone")
                .HasConversion(dateOnlyConverter);
            entity
                .Property(o => o.AdvanceOrderDate)
                .HasColumnType("timestamp without time zone")
                .HasConversion(dateOnlyConverter);
            entity
                .HasMany(o => o.OrderDetails)
                .WithOne(od => od.OrderMaster)
                .HasForeignKey(od => od.OrderMasterId)
                .HasPrincipalKey(o => o.OrderMasterId);
        });

        modelBuilder.Entity<OrderModeCompanyMapping>(entity =>
        {
            entity.HasKey(e => e.OrderModeMappingId);

            entity.ToTable("order_mode_company_mapping");
        });

        modelBuilder.Entity<PaymentMode>(entity =>
        {
            entity.ToTable("payment_mode");

            entity.Property(e => e.PaymentMode1).HasColumnName("PaymentMode");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("product");
        });

        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId);

            entity.ToTable("product_category");
        });

        modelBuilder.Entity<ProductDetail>(entity =>
        {
            entity.ToTable("product_detail");
        });

        modelBuilder.Entity<ProductDetailAvailability>(entity =>
        {
            entity.HasKey(e => e.ProductDetailAvailableId);

            entity.ToTable("product_detail_availability");
        });

        modelBuilder.Entity<ProductDetailBranchMapping>(entity =>
        {
            entity.ToTable("product_detail_branch_mapping");
        });

        modelBuilder.Entity<ProductDetailOrderSourcePriceMapping>(entity =>
        {
            entity.HasKey(e => e.MapId);

            entity.ToTable("product_detail_order_source_price_mapping");
        });

        modelBuilder.Entity<ProductSize>(entity =>
        {
            entity.HasKey(e => e.SizeId);

            entity.ToTable("product_size");
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

        modelBuilder.Entity<BranchOrderSequence>(entity =>
        {
            entity.HasKey(e => e.BranchId);
            entity.ToTable("branch_order_sequence");
        });

        modelBuilder.Entity<UserLogin>(entity =>
        {
            entity.HasKey(e => e.UserId);
            entity.ToTable("user_login");
        });

        modelBuilder.Entity<UserBranchMapping>(entity =>
        {
            entity.HasKey(e => e.UserBranchId);
            entity.ToTable("user_branch_mapping");
            entity.Property(x => x.UserId)
            .HasColumnName("UserID");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.RoleId);
            entity.ToTable("user_role");
        });

        modelBuilder.Entity<OrderStatus>(entity =>
        {
            entity.ToTable("order_status");
            entity.HasKey(x => x.OrderStatusId);
            entity
                .Property(x => x.OrderStatusName)
                .HasColumnName("OrderStatus");
        });

        modelBuilder.Entity<OrderStatusLog>(entity =>
        {
            entity.ToTable("order_status_log");
            entity.HasKey(x => x.OrderStatusLogId);
        });

        modelBuilder.Entity<Rider>(entity =>
        {
            entity.ToTable("rider");
            entity.HasKey(x => x.RiderId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

// Add this class to your file or in a suitable shared location if you are targeting .NET 6 or earlier
// (DateOnlyToDateTimeConverter is not available in EF Core < 7.0)
public class DateOnlyToDateTimeConverter : ValueConverter<DateOnly, DateTime>
{
    public DateOnlyToDateTimeConverter() : base(
        d => d.ToDateTime(TimeOnly.MinValue),
        d => DateOnly.FromDateTime(d))
    { }
}
