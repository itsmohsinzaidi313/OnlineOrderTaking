using ExportService.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExportService.DatabaseContexts
{
    public class PostgresDbContext(DbContextOptions<PostgresDbContext> options) : DbContext(options)
    {
        public DbSet<SetupCompany> SetupCompanies => Set<SetupCompany>();
        public DbSet<BranchMaster> BranchMasters => Set<BranchMaster>();
        public DbSet<City> Cities => Set<City>();
        public DbSet<Area> Areas => Set<Area>();
        public DbSet<BranchDetail> BranchDetails => Set<BranchDetail>();
        public DbSet<BranchDayMapping> BranchDayMappings => Set<BranchDayMapping>();
        public DbSet<SetupMaster> SetupMasters => Set<SetupMaster>();
        public DbSet<SetupMasterDetail> SetupMasterDetails => Set<SetupMasterDetail>();
        public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<ProductDetail> ProductDetails => Set<ProductDetail>();
        public DbSet<ProductSize> ProductSizes => Set<ProductSize>();
        public DbSet<Flavour> Flavours => Set<Flavour>();
        public DbSet<DealItemDetail> DealItemDetails => Set<DealItemDetail>();
        public DbSet<DealDescription> DealDescriptions => Set<DealDescription>();
        public DbSet<CategoryAvailability> CategoryAvailabilities => Set<CategoryAvailability>();
        public DbSet<PaymentMode> PaymentModes => Set<PaymentMode>();
        public DbSet<GST> GSTs => Set<GST>();
        public DbSet<OrderModeCompanyMapping> OrderModeCompanyMappings => Set<OrderModeCompanyMapping>();
        public DbSet<ProductDetailBranchMapping> ProductDetailBranchMappings => Set<ProductDetailBranchMapping>();
        public DbSet<ProductDetailAvailability> ProductDetailAvailabilities => Set<ProductDetailAvailability>();
        public DbSet<ProductDetailOrderSourcePriceMapping> ProductDetailOrderSourcePriceMappings => Set<ProductDetailOrderSourcePriceMapping>();
        public DbSet<Discount> Discounts => Set<Discount>();
        public DbSet<DiscountDayMapping> DiscountDayMappings => Set<DiscountDayMapping>();
        public DbSet<DiscountProductDetailMapping> DiscountProductDetailMappings => Set<DiscountProductDetailMapping>();
        public DbSet<DiscountBranchMapping> DiscountBranchMappings => Set<DiscountBranchMapping>();
        public DbSet<DiscountOrderTypeMapping> DiscountOrderTypeMappings => Set<DiscountOrderTypeMapping>();
        public DbSet<DiscountOrderModeMapping> DiscountOrderModeMappings => Set<DiscountOrderModeMapping>();
        public DbSet<SetupCompanySetting> SetupCompanySettings => Set<SetupCompanySetting>();
        public DbSet<OrderMaster> OrderMasters => Set<OrderMaster>();
        public DbSet<OrderDetail> OrderDetails => Set<OrderDetail>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<CustomerPhone> CustomerPhones => Set<CustomerPhone>();
        public DbSet<CustomerAddressDetail> CustomerAddressDetails => Set<CustomerAddressDetail>();
        public DbSet<BranchOrderSequence> BranchOrderSequences => Set<BranchOrderSequence>();
        public DbSet<UserLogin> UserLogins => Set<UserLogin>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();
        public DbSet<UserBranchMapping> UserBranchMappings => Set<UserBranchMapping>();
        public DbSet<OrderStatus> OrderStatuses => Set<OrderStatus>();
        public DbSet<OrderStatusLog> OrderStatusLogs => Set<OrderStatusLog>();
        public DbSet<Rider> Riders => Set<Rider>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SetupCompany>()
                .ToTable("setup_company")
                .HasKey(x => x.CompanyId);

            modelBuilder.Entity<BranchMaster>()
                .ToTable("branch_master")
                .HasKey(x => x.BranchId);

            modelBuilder.Entity<City>()
                .ToTable("city")
                .HasKey(x => x.CityId);

            modelBuilder.Entity<Area>()
                .ToTable("area")
                .HasKey(x => x.AreaId);

            modelBuilder.Entity<BranchDetail>()
                .ToTable("branch_detail")
                .HasKey(x => x.BranchDetailId);

            modelBuilder.Entity<BranchDayMapping>()
                .ToTable("branch_day_mapping")
                .HasKey(x => x.BranchDayMappingId);

            modelBuilder.Entity<SetupMaster>()
                .ToTable("setup_master")
                .HasKey(x => x.SetupMasterId);

            modelBuilder.Entity<SetupMasterDetail>()
                .ToTable("setup_master_detail")
                .HasKey(x => x.SetupDetailId);

            modelBuilder.Entity<ProductCategory>()
                .ToTable("product_category")
                .HasKey(x => x.CategoryId);

            modelBuilder.Entity<Product>()
                .ToTable("product")
                .HasKey(x => x.ProductId);

            modelBuilder.Entity<ProductDetail>()
                .ToTable("product_detail")
                .HasKey(x => x.ProductDetailId);

            modelBuilder.Entity<ProductSize>()
                .ToTable("product_size")
                .HasKey(x => x.SizeId);

            modelBuilder.Entity<Flavour>()
                .ToTable("flavour")
                .HasKey(x => x.FlavourId);

            modelBuilder.Entity<DealItemDetail>()
                .ToTable("deal_item_detail")
                .HasKey(x => x.DealItemId);

            modelBuilder.Entity<DealDescription>()
                .ToTable("deal_description")
                .HasKey(x => x.DealDescId);

            modelBuilder.Entity<CategoryAvailability>()
                .ToTable("category_availability")
                .HasKey(x => x.CategoryAvailableId);

            modelBuilder.Entity<PaymentMode>()
                .ToTable("payment_mode")
                .HasKey(x => x.PaymentModeId);

            modelBuilder.Entity<PaymentMode>()
                .Property(x => x.PaymentModeName)
                .HasColumnName("PaymentMode");

            modelBuilder.Entity<GST>()
                .ToTable("gst")
                .HasKey(x => x.GSTId);

            modelBuilder.Entity<OrderModeCompanyMapping>()
                .ToTable("order_mode_company_mapping")
                .HasKey(x => x.OrderModeMappingId);

            modelBuilder.Entity<ProductDetailBranchMapping>()
                .ToTable("product_detail_branch_mapping")
                .HasKey(x => x.ProductDetailBranchMappingId);

            modelBuilder.Entity<ProductDetailAvailability>()
                .ToTable("product_detail_availability")
                .HasKey(x => x.ProductDetailAvailableId);

            modelBuilder.Entity<ProductDetailOrderSourcePriceMapping>()
                .ToTable("product_detail_order_source_price_mapping")
                .HasKey(x => x.MapId);

            modelBuilder.Entity<Discount>()
                .ToTable("discount")
                .HasKey(x => x.DiscountId);

            modelBuilder.Entity<Discount>()
                .Property(x => x.DiscountTimeStart)
                .HasColumnType("time without time zone");

            modelBuilder.Entity<Discount>()
                .Property(x => x.DiscountTimeEnd)
                .HasColumnType("time without time zone");

            modelBuilder.Entity<Discount>()
                .Property(x => x.StartDate)
                .HasColumnType("timestamp without time zone");

            modelBuilder.Entity<Discount>()
                .Property(x => x.EndDate)
                .HasColumnType("timestamp without time zone");

            modelBuilder.Entity<DiscountDayMapping>()
                .ToTable("discount_day_mapping")
                .HasKey(x => x.DiscountDayMappingId);

            modelBuilder.Entity<DiscountProductDetailMapping>()
                .ToTable("discount_product_detail_mapping")
                .HasKey(x => x.DiscountProductDetailMappingId);

            modelBuilder.Entity<DiscountBranchMapping>()
                .ToTable("discount_branch_mapping")
                .HasKey(x => x.DiscountBranchMappingId);

            modelBuilder.Entity<DiscountOrderTypeMapping>()
                .ToTable("discount_order_type_mapping")
                .HasKey(x => x.DiscountOrderTypeMappingId);

            modelBuilder.Entity<DiscountOrderModeMapping>()
                .ToTable("discount_order_mode_mapping")
                .HasKey(x => x.DiscountOrderModeMappingId);

            modelBuilder.Entity<SetupCompanySetting>()
                .ToTable("setup_company_setting")
                .HasKey(x => x.SettingId);

            modelBuilder.Entity<BranchOrderSequence>()
                .ToTable("branch_order_sequence")
                .HasKey(x => x.BranchId);

            modelBuilder.Entity<Customer>()
                .ToTable("customer")
                .HasKey(x => x.CustomerId);

            modelBuilder.Entity<CustomerAddressDetail>()
                .ToTable("customer_address_detail")
                .HasKey(x => x.CustomerAddressId);

            modelBuilder.Entity<CustomerAddressDetail>()
                .Ignore(x => x.CreatedDate)
                .Ignore(x => x.CreatedBy);

            modelBuilder.Entity<CustomerPhone>()
                .ToTable("customer_phone")
                .HasKey(x => x.PhoneId);

            modelBuilder.Entity<CustomerPhone>()
                .Ignore(x => x.CreatedDate)
                .Ignore(x => x.CreatedBy);

            modelBuilder.Entity<OrderMaster>()
                .ToTable("order_master")
                .HasKey(x => x.OrderMasterId);

            modelBuilder.Entity<OrderMaster>()
                .HasIndex(x => x.OrderToken)
                .IsUnique();

            modelBuilder.Entity<OrderMaster>()
                .HasMany(o => o.OrderDetails)
                .WithOne(od => od.OrderMaster)
                .HasForeignKey(od => od.OrderMasterId)
                .HasPrincipalKey(o => o.OrderMasterId);

            modelBuilder.Entity<OrderDetail>()
                .Ignore(x => x.CreatedDate)
                .Ignore(x => x.CreatedBy);

            modelBuilder.Entity<OrderMaster>()
                .Property(o => o.OrderDate)
                .HasColumnType("timestamp without time zone");

            modelBuilder.Entity<OrderMaster>()
                .Property(o => o.AdvanceOrderDate)
                .HasColumnType("timestamp without time zone");

            modelBuilder.Entity<OrderMaster>()
                .Ignore(x => x.CreatedDate);

            modelBuilder.Entity<OrderDetail>()
                .ToTable("order_detail")
                .HasKey(x => x.OrderDetailId);

            modelBuilder.Entity<UserLogin>()
                .ToTable("user_login")
                .HasKey(x => x.UserId);

            modelBuilder.Entity<UserLogin>()
                .Property(x => x.IsEnabled)
                .HasColumnName("IsEnable");

            modelBuilder.Entity<UserRole>()
                .ToTable("user_role")
                .HasKey(x => x.RoleId);

            modelBuilder.Entity<UserBranchMapping>()
                .ToTable("user_branch_mapping")
                .HasKey(x => x.UserBranchId);

            modelBuilder.Entity<UserBranchMapping>()
                .Property(x => x.UserId)
                .HasColumnName("UserID");

            modelBuilder.Entity<OrderStatus>()
                .ToTable("order_status")
                .HasKey(x => x.OrderStatusId);

            modelBuilder.Entity<OrderStatus>()
                .Property(x => x.OrderStatusName)
                .HasColumnName("OrderStatus");

            modelBuilder.Entity<OrderStatusLog>()
            .ToTable("order_status_log")
            .HasKey(x => x.OrderStatusLogId);

            modelBuilder.Entity<Rider>()
                .ToTable("rider")
                .HasKey(x => x.RiderId);

            modelBuilder.Entity<OrderStatusLog>()
                .Ignore(x => x.CreatedBy)
                .Ignore(x => x.IsActive);
        }
    }
}
