using ImportService.Data;
using ImportService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImportService
{
    public class Implementation(
        RestaurantsDbContext pgDb,
        ILogger<Implementation> logger,
        ISetupCompanyMigrationService service_setupCompany,
        IBranchMasterMigrationService service_branchMaster,
        IMenuMigrationService service_menu,
        ISetupMasterMigrationService service_setupMaster,
        ISetupMasterDetailMigrationService service_setupMasterDetail,
        IPaymentModeMigrationService service_paymentMode,
        IDiscountMigrationService service_discount,
        IProductSizeMigrationService service_productSize,
        IFlavourMigrationService service_flavour,
        ICityMigrationService service_city,
        IAreaMigrationService service_area,
        ISetupCompanySettingsMigrationService service_setupCompanySettings,
        ICustomerDataImportService service_customerData,
        IGSTMigrationService service_gst,
        IOrdersImportService service_orders)
    {
        private const string PostgresHost = "haproxy";
        public async Task<IResult?> Import(int companyId, string dbName, CancellationToken cancellationToken = default)
        {
            //try
            //{
                var isNewRestaurant = await RestaurantCreated(dbName, cancellationToken);
                var postgresDbContext = GetPgDbContext($"Host={PostgresHost};Port=5433;Database={dbName};Username=postgres;Password=postgrespass");

                var dbCreated = await postgresDbContext.Database.EnsureCreatedAsync(cancellationToken);
                if (!dbCreated)
                {
                    var orderMaster = await postgresDbContext.OrderMasters.FirstOrDefaultAsync(cancellationToken);
                    if (orderMaster != null) return Results.BadRequest("Order master has data");
                }

                await service_setupCompany.MigrateSetupCompanyAsync(companyId, postgresDbContext, cancellationToken);

                await service_setupMaster.MigrateSetupMasterAsync(postgresDbContext, cancellationToken);

                await service_setupMasterDetail.MigrateSetupMasterDetailAsync(companyId, postgresDbContext, cancellationToken);

                await service_city.MigrateCitiesAsync(postgresDbContext, cancellationToken);

                await service_area.MigrateAreasAsync(companyId, postgresDbContext, cancellationToken);

                await service_branchMaster.MigrateBranchMasterAsync(companyId, postgresDbContext, cancellationToken);

                await service_productSize.MigrateProductSizesAsync(companyId, postgresDbContext, cancellationToken);

                await service_flavour.MigrateFlavoursAsync(companyId, postgresDbContext, cancellationToken);

                await service_menu.MigrateMenuAsync(companyId, postgresDbContext, cancellationToken);

                await service_paymentMode.MigratePaymentModesAsync(companyId, postgresDbContext, cancellationToken);

                await service_setupCompanySettings.MigrateSetupCompanySettingsAsync(companyId, postgresDbContext, cancellationToken);

                await service_discount.MigrateDiscountsAsync(companyId, postgresDbContext, cancellationToken);

                await service_customerData.MigrateCustomerDataAsync(companyId, postgresDbContext, cancellationToken);

                await service_gst.MigrateGSTsAsync(companyId, postgresDbContext, cancellationToken);

                await service_orders.MigrateOrdersAsync(companyId, postgresDbContext, cancellationToken);

                await postgresDbContext.SaveChangesAsync(cancellationToken);

                logger.LogInformation("Data import completed successfully for database: {DbName}", dbName);
                return Results.Ok("Import completed successfully");
            //}
            //catch (Exception ex)
            //{
            //    logger.LogError(ex, "Error occurred while importing data");
            //    return Results.Problem(ex.InnerException?.Message ?? ex.Message, statusCode: 500);
            //}
        }

        private async Task<bool> RestaurantCreated(string dbName, CancellationToken cancellationToken)
        {
            var restaurant = await pgDb.Restaurants.FirstOrDefaultAsync(x => x.DomainName.Contains(dbName), cancellationToken);
            if (restaurant == null)
            {
                restaurant = new Entities.Restaurants
                {
                    DomainName = dbName,
                    ConnectionString = $"Host=haproxy;Port=5434;Database={dbName};Username=postgres;Password=postgrespass",
                    Name = dbName
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
