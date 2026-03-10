using ImportService.Data;
using ImportService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImportService
{
    public class Implementation(
        IDbContextFactory<RestaurantsDbContext> pgDbFactory,
        ILogger<Implementation> logger,
        ISetupCompanyMigrationService setupCompany,
        IBranchMasterMigrationService branchMaster,
        IMenuMigrationService menu,
        ISetupMasterMigrationService setupMaster,
        ISetupMasterDetailMigrationService setupMasterDetail,
        IPaymentModeMigrationService paymentMode,
        IDiscountMigrationService discount,
        IProductSizeMigrationService productSize,
        IFlavourMigrationService flavour,
        ICityMigrationService city,
        IAreaMigrationService area,
        ISetupCompanySettingsMigrationService setupCompanySettings,
        ICustomerDataImportService customerData,
        IGSTMigrationService gst,
        IUserLoginMigrationService userLogin,
        IRidersMigrationService riders)
    {
        private const string PostgresHost = "haproxy";
        public async Task<IResult?> Import(int companyId, string domainName, bool checkOrders, CancellationToken cancellationToken = default)
        {
            try
            {
                var isNewRestaurant = await RestaurantCreated(domainName, cancellationToken);
                var dbname = domainName.Split(".")[0];
                using var postgresDbContext = GetPgDbContext($"Host={PostgresHost};Port=5433;Database={dbname};Username=postgres;Password=postgrespass");

                var dbCreated = await postgresDbContext.Database.EnsureCreatedAsync(cancellationToken);
                if (!dbCreated && checkOrders)
                {
                    var orderMaster = await postgresDbContext.OrderMasters.FirstOrDefaultAsync(cancellationToken);
                    if (orderMaster != null) return Results.BadRequest("Order master has data");
                }

                await setupCompany.MigrateAsync(postgresDbContext, companyId, cancellationToken);

                await setupMaster.MigrateAsync(postgresDbContext, ct: cancellationToken);

                await setupMasterDetail.MigrateAsync(postgresDbContext, companyId, cancellationToken);

                await city.MigrateAsync(postgresDbContext, ct: cancellationToken);

                await area.MigrateAsync(postgresDbContext, companyId, cancellationToken);

                await branchMaster.MigrateAsync(postgresDbContext, companyId, cancellationToken);

                await productSize.MigrateAsync(postgresDbContext, companyId, cancellationToken);

                await flavour.MigrateAsync(postgresDbContext, companyId, cancellationToken);

                await menu.MigrateAsync(postgresDbContext, companyId, cancellationToken);

                await paymentMode.MigrateAsync(postgresDbContext, companyId, cancellationToken);

                await setupCompanySettings.MigrateAsync(postgresDbContext, companyId, cancellationToken);

                await discount.MigrateAsync(postgresDbContext, companyId, cancellationToken);

                await customerData.MigrateAsync(postgresDbContext, companyId, cancellationToken);

                await gst.MigrateAsync(postgresDbContext, companyId, cancellationToken);

                await userLogin.MigrateAsync(postgresDbContext, companyId, cancellationToken);

                await riders.MigrateAsync(postgresDbContext, companyId, cancellationToken);

                await postgresDbContext.SaveChangesAsync(cancellationToken);

                logger.LogInformation("Data import completed successfully for database: {DbName}", dbname);
                return Results.Ok("Import completed successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while importing data");
                return Results.Problem(ex.InnerException?.Message ?? ex.Message, statusCode: 500);
            }
        }

        private async Task<bool> RestaurantCreated(string domain, CancellationToken cancellationToken)
        {
            using var pgDb = pgDbFactory.CreateDbContext();
            await pgDb.Database.EnsureCreatedAsync(cancellationToken);
            var restaurant = await pgDb.Restaurants.FirstOrDefaultAsync(x => x.DomainName == domain, cancellationToken);
            if (restaurant == null)
            {
                var dbname = domain.Split(".")[0];
                restaurant = new Entities.Restaurants
                {
                    DomainName = domain,
                    ConnectionString = $"Host=haproxy;Port=5434;Database={dbname};Username=postgres;Password=postgrespass",
                    Name = dbname
                };
                await pgDb.Restaurants.AddAsync(restaurant, cancellationToken);
                await pgDb.SaveChangesAsync(cancellationToken);
                return true;
            }
            return false;
        }

        private static PostgresDbContext GetPgDbContext(string connectionString)
        {
            var options = new DbContextOptionsBuilder<PostgresDbContext>()
                .UseNpgsql(connectionString, options =>
                {
                    options.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorCodesToAdd: null);
                })
                .Options;
            return new PostgresDbContext(options);
        }
    }
}
