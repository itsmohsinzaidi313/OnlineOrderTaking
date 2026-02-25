using ImportService.Entities;
using Microsoft.EntityFrameworkCore;

namespace ImportService.Data
{
    public class SqlServerDbContext(DbContextOptions<SqlServerDbContext> options) : DbContext(options)
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
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<CustomerPhone> CustomerPhones => Set<CustomerPhone>();
        public DbSet<CustomerAddressDetail> CustomerAddressDetails => Set<CustomerAddressDetail>();
        public DbSet<OrderMaster> OrderMasters => Set<OrderMaster>();
        public DbSet<OrderDetail> OrderDetails => Set<OrderDetail>();
        public DbSet<UserLogin> UserLogins => Set<UserLogin>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();
        public DbSet<UserBranchMapping> UserBranchMappings => Set<UserBranchMapping>();
        public DbSet<OrderStatus> OrderStatuses => Set<OrderStatus>();
        public DbSet<Rider> Riders => Set<Rider>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SetupCompany>()
                .ToTable("SetupCompany")
                .HasKey(x => x.CompanyId);

            modelBuilder.Entity<BranchMaster>()
                .Ignore(b => b.CityName)
                .ToTable("BranchMaster", "dbo")
                .HasKey(x => x.BranchId);

            modelBuilder.Entity<City>()
                .ToTable("City", "dbo")
                .HasKey(x => x.CityId);

            modelBuilder.Entity<Area>()
                .ToTable("Area", "dbo")
                .HasKey(x => x.AreaId);

            modelBuilder.Entity<BranchDetail>()
                .Ignore(b => b.AreaName)
                .Ignore(b => b.AreaStartTime)
                .Ignore(b => b.AreaEndTime)
                .ToTable("BranchDetail", "dbo")
                .HasKey(x => x.BranchDetailId);

            modelBuilder.Entity<SetupMaster>()
                .ToTable("Setup_Master", "dbo")
                .HasKey(x => x.SetupMasterId);

            modelBuilder.Entity<SetupMasterDetail>()
                .ToTable("Setup_MasterDetail", "dbo")
                .HasKey(x => x.SetupDetailId);

            modelBuilder.Entity<ProductCategory>()
                .ToTable("ProductCategory", "dbo")
                .HasKey(x => x.CategoryId);

            modelBuilder.Entity<Product>()
                .ToTable("Product", "dbo")
                .HasKey(x => x.ProductId);

            modelBuilder.Entity<ProductDetail>()
                .Ignore(b => b.SizeName)
                .Ignore(b => b.FlavourName)
                .ToTable("ProductDetail", "dbo")
                .HasKey(x => x.ProductDetailId);

            modelBuilder.Entity<ProductSize>()
                .ToTable("ProductSize", "dbo")
                .HasKey(x => x.SizeId);

            modelBuilder.Entity<Flavour>()
                .ToTable("Flavour", "dbo")
                .HasKey(x => x.FlavourId);

            modelBuilder.Entity<DealItemDetail>()
                .ToTable("DealItemDetail", "dbo")
                .HasKey(x => x.DealItemId);

            modelBuilder.Entity<DealDescription>()
                .ToTable("DealDescription", "dbo")
                .HasKey(x => x.DealDescId);

            modelBuilder.Entity<CategoryAvailability>()
                .ToTable("CategoryAvailability", "dbo")
                .HasKey(x => x.CategoryAvailableId);

            modelBuilder.Entity<PaymentMode>()
                .ToTable("PaymentMode", "dbo")
                .HasKey(x => x.PaymentModeId);

            modelBuilder.Entity<PaymentMode>()
                .Property(x => x.PaymentModeName)
                .HasColumnName("PaymentMode");

            modelBuilder.Entity<GST>()
                .ToTable("GST", "dbo")
                .HasKey(x => x.GSTId);

            modelBuilder.Entity<OrderModeCompanyMapping>()
                .ToTable("OrderModeCompanyMapping", "dbo")
                .HasKey(x => x.OrderModeMappingId);

            modelBuilder.Entity<ProductDetailBranchMapping>()
                .ToTable("ProductDetailBranchMapping", "dbo")
                .HasKey(x => x.ProductDetailBranchMappingId);

            modelBuilder.Entity<ProductDetailAvailability>()
                .ToTable("ProductDetailAvailability", "dbo")
                .HasKey(x => x.ProductDetailAvailableId);

            modelBuilder.Entity<ProductDetailOrderSourcePriceMapping>()
                .ToTable("ProductDetailOrderSourcePriceMapping", "dbo")
                .HasKey(x => x.MapId);

            modelBuilder.Entity<Discount>()
                 .ToTable("Discount", "dbo")
                 .HasKey(x => x.DiscountId);

            modelBuilder.Entity<Discount>()
                .Property(x => x.DiscountTimeStart)
                .HasColumnType("time(7)");

            modelBuilder.Entity<Discount>()
                .Property(x => x.DiscountTimeEnd)
                .HasColumnType("time(7)");

            modelBuilder.Entity<Discount>()
                .Property(x => x.StartDate)
                .HasColumnType("datetime");

            modelBuilder.Entity<Discount>()
                .Property(x => x.EndDate)
                .HasColumnType("datetime");

            modelBuilder.Entity<DiscountDayMapping>()
                .ToTable("DiscountDayMapping", "dbo")
                .HasKey(x => x.DiscountDayMappingId);

            modelBuilder.Entity<DiscountProductDetailMapping>()
                .ToTable("DiscountProductDetailMapping", "dbo")
                .HasKey(x => x.DiscountProductDetailMappingId);

            modelBuilder.Entity<DiscountBranchMapping>()
                .ToTable("DiscountBranchMapping", "dbo")
                .HasKey(x => x.DiscountBranchMappingId);

            modelBuilder.Entity<DiscountOrderTypeMapping>()
                .ToTable("DiscountOrderTypeMapping", "dbo")
                .HasKey(x => x.DiscountOrderTypeMappingId);

            modelBuilder.Entity<DiscountOrderModeMapping>()
                .ToTable("DiscountOrderModeMapping", "dbo")
                .HasKey(x => x.DiscountOrderModeMappingId);

            modelBuilder.Entity<SetupCompanySetting>()
                .ToTable("SetupCompanySetting", "dbo")
                .HasKey(x => x.SettingId);

            modelBuilder.Entity<Customer>()
                .ToTable("Customer")
                .HasKey(x => x.CustomerId);

            modelBuilder.Entity<CustomerPhone>()
                .ToTable("CustomerPhone")
                .HasKey(x => x.PhoneId);

            modelBuilder.Entity<CustomerAddressDetail>()
                .ToTable("CustomerAddressDetail")
                .HasKey(x => x.CustomerAddressId);

            modelBuilder.Entity<CustomerPhone>()
                .HasMany(c => c.Customers)
                .WithOne(p => p.CustomerPhone)
                .HasPrincipalKey(p => p.PhoneId)
                .HasForeignKey(x => x.PhoneId);

            modelBuilder.Entity<CustomerPhone>()
                .HasMany(x => x.CustomerAddressDetails)
                .WithOne(x => x.CustomerPhone)
                .HasPrincipalKey(x => x.PhoneId)
                .HasForeignKey(x => x.PhoneId);

            modelBuilder.Entity<Customer>()
                .HasOne(c => c.CustomerPhone)
                .WithMany(p => p.Customers)
                .HasForeignKey(c => c.PhoneId)
                .HasPrincipalKey(p => p.PhoneId);

            modelBuilder.Entity<CustomerAddressDetail>()
                .HasOne(c => c.CustomerPhone)
                .WithMany(p => p.CustomerAddressDetails)
                .HasForeignKey(c => c.PhoneId)
                .HasPrincipalKey(p => p.PhoneId);

            modelBuilder.Entity<OrderMaster>()
                .ToTable("OrderMaster")
                .HasKey(x => x.OrderMasterId);

            modelBuilder.Entity<OrderDetail>()
                .ToTable("OrderDetail")
                .HasKey(x => x.OrderDetailId);

            modelBuilder.Entity<UserLogin>()
                .ToTable("UserLogin")
                .HasKey(x => x.UserId);

            modelBuilder.Entity<UserRole>()
                .ToTable("UserRole")
                .HasKey(x => x.RoleId);

            modelBuilder.Entity<UserBranchMapping>()
                .ToTable("UserBranchMapping")
                .HasKey(x => x.UserBranchId);

            modelBuilder.Entity<UserBranchMapping>()
                .Property(x => x.UserId)
                .HasColumnName("UserID");

            modelBuilder.Entity<UserLogin>()
                .Property(x => x.IsEnabled)
                .HasColumnName("IsEnable");

            modelBuilder.Entity<OrderStatus>()
                .ToTable("OrderStatus")
                .HasKey(x => x.OrderStatusId);

            modelBuilder.Entity<OrderStatus>()
                .Property(x => x.OrderStatusName)
                .HasColumnName("OrderStatus");

            modelBuilder.Entity<Rider>()
                .ToTable("Rider")
                .HasKey(x => x.RiderId);
        }
    }
}
