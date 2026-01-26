using ImportService.Data;
using ImportService.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportService
{
    public class Implementation(
        SqlServerDbContext sqlServerDbContext,
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
        ISetupCompanySettingsMigrationService service_setupCompanySettings)
    {
        public async Task<IResult?> Import(int companyId, CancellationToken cancellationToken = default)
        {
            try
            {
                var company = await sqlServerDbContext.SetupCompanies.FirstOrDefaultAsync(x => x.CompanyId == companyId, cancellationToken);
                if (company == null)
                {
                    return Results.NotFound("Company not found");
                }

                var url = company.WebsiteUrl;
                if (url == null)
                {
                    return Results.NotFound("Company website URL not found");
                }
                var domain = url.Replace("http://", "").Replace("https://", "").Replace("www.", "").Split('/')[0];
                var dbName = domain.Split('.')[0];
                var isNewRestaurant = await RestaurantCreated(domain, cancellationToken);
                var postgresDbContext = GetPgDbContext($"Host=haproxy;Port=5434;Database={dbName};Username=postgres;Password=postgrespass");

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
                await postgresDbContext.SaveChangesAsync(cancellationToken);
                return Results.Ok("Import completed successfully");
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.InnerException?.Message ?? ex.Message, statusCode: 500);
            }
        }

        private static async Task<bool> RestaurantCreated(string domain, CancellationToken cancellationToken)
        {
            var pgDb = GetRestaurantsDbContext("Host=haproxy;Port=5434;Database=restaurants;Username=postgres;Password=postgrespass");
            var restaurant = await pgDb.Restaurants.FirstOrDefaultAsync(x => x.DomainName == domain, cancellationToken);
            if (restaurant == null)
            {
                var dbName = domain.Split('.')[0];
                restaurant = new Entities.Restaurants
                {
                    DomainName = domain,
                    ConnectionString = $"Host=haproxy;Port=5434;Database={dbName};Username=postgres;Password=postgrespass",
                    Name = domain
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

        private static RestaurantsDbContext GetRestaurantsDbContext(string connectionString)
        {
            var options = new DbContextOptionsBuilder<RestaurantsDbContext>()
                .UseNpgsql(connectionString, options =>
                {
                    options.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorCodesToAdd: null);
                })
                .Options;
            return new RestaurantsDbContext(options);
        }
    }
}
