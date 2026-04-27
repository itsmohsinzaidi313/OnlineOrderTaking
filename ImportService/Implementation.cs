using ImportService.DatabaseContexts;
using ImportService.Interfaces;
using Microsoft.EntityFrameworkCore;
using Nager.PublicSuffix;
using Nager.PublicSuffix.RuleProviders;

namespace ImportService
{
    public class Implementation(
        IDbContextFactory<SqlServerDbContext> sqlDbFactory,
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
        IGSTMigrationService gst,
        IUserLoginMigrationService userLogin,
        IRidersMigrationService riders,
        IOrderModeCompanyMappingMigrationService omcm)
    {
        public async Task<IResult?> Import(int companyId, string selection, CancellationToken cancellationToken = default)
        {
            try
            {
                var ruleProvider = new SimpleHttpRuleProvider();
                await ruleProvider.BuildAsync(cancellationToken: cancellationToken);
                var webSiteUrl = await GetCompanyWebsiteUrl(companyId, cancellationToken);
                var domainInfo = ExtractDomainInfo(webSiteUrl, ruleProvider);
                var dbName = domainInfo.Domain == "eatx" ? domainInfo.Subdomain : domainInfo.Domain;

                using var postgresDbContext = GetPgDbContext($"Host=haproxy;Port=5433;Database={dbName};Username=postgres;Password=postgrespass");

                var isNewRestaurant = await postgresDbContext.Database.EnsureCreatedAsync(cancellationToken);
                if (isNewRestaurant)
                {
                    await AddRestaurantEntry(domainInfo, cancellationToken);
                    logger?.LogInformation("Database {DbName} created successfully.", dbName);
                }
                else
                {
                    logger?.LogInformation("Database {DbName} already exists.", dbName);
                }

                Dictionary<string, IMigrationService> migrationServices = new()
                {
                    { "setupCompany", setupCompany },
                    { "setupMaster", setupMaster },
                    { "setupMasterDetail", setupMasterDetail },
                    { "city", city },
                    { "area", area },
                    { "branchMaster", branchMaster },
                    { "productSize", productSize },
                    { "flavour", flavour },
                    { "menu", menu },
                    { "paymentMode", paymentMode },
                    { "setupCompanySettings", setupCompanySettings },
                    { "discount", discount },
                    { "gst", gst },
                    { "userLogin", userLogin },
                    { "riders", riders },
                    { "orderMode", omcm  }
                };

                if (selection == "all")
                {
                    foreach (var service in migrationServices.Values)
                    {
                        await service.MigrateAsync(postgresDbContext, companyId, cancellationToken);
                    }
                }
                else
                {
                    var service = migrationServices[selection];
                    await service.MigrateAsync(postgresDbContext, companyId, cancellationToken);
                }

                await postgresDbContext.SaveChangesAsync(cancellationToken);

                logger?.LogInformation("Data import completed successfully for database: {DbName}", dbName);
                return Results.Ok($"Import completed successfully for {dbName}");
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error occurred while importing data");
                return Results.Problem(ex.InnerException?.Message ?? ex.Message, statusCode: 500);
            }
        }

        static private DomainInfo ExtractDomainInfo(string webSiteUrl, SimpleHttpRuleProvider ruleProvider)
        {
            var domainParser = new DomainParser(ruleProvider);
            var domainInfo = domainParser.Parse(webSiteUrl);
            return domainInfo ?? throw new InvalidOperationException("Failed to parse domain from URL.");
        }

        private async Task<string> GetCompanyWebsiteUrl(int companyId, CancellationToken cancellationToken)
        {
            var sqlServerDbContext = sqlDbFactory.CreateDbContext();
            var company = await sqlServerDbContext.SetupCompanies.FirstOrDefaultAsync(x => x.CompanyId == companyId, cancellationToken);
            if (company == null)
            {
                throw new InvalidOperationException($"Company with ID {companyId} not found.");
            }
            if (string.IsNullOrEmpty(company.WebsiteUrl))
            {
                throw new InvalidOperationException($"Company with ID {companyId} does not have a website URL.");
            }
            return company.WebsiteUrl.Trim();
        }

        private async Task<bool> AddRestaurantEntry(DomainInfo domainInfo, CancellationToken cancellationToken)
        {
            var dbName = domainInfo.Domain == "eatx" ? domainInfo.Subdomain : domainInfo.Domain;

            using var pgDb = pgDbFactory.CreateDbContext();
            await pgDb.Database.EnsureCreatedAsync(cancellationToken);
            var restaurant = await pgDb.Restaurants.FirstOrDefaultAsync(x => x.DomainName == domainInfo.FullyQualifiedDomainName, cancellationToken);
            if (restaurant == null)
            {
                restaurant = new Entities.Restaurants
                {
                    DomainName = dbName,
                    ConnectionString = $"Host=haproxy;Port=5433;Database={dbName};Username=postgres;Password=postgrespass",
                    Name = domainInfo.FullyQualifiedDomainName
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
