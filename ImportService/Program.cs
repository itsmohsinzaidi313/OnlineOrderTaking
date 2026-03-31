using ImportService;
using ImportService.Data;
using ImportService.Interfaces;
using ImportService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configuration
builder.Configuration
    .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Connection strings
var sqlServerConnectionString =
    builder.Configuration.GetConnectionString("SqlServer")
    ?? throw new InvalidOperationException("SqlServer connection string is not configured.");

var postgresConnectionString = builder.Configuration.GetConnectionString("Postgres")
    ?? throw new InvalidOperationException("Postgres connection string is not configured.");
// Services

builder.Services
    .AddDbContextFactory<SqlServerDbContext>(options =>
        options.UseSqlServer(
            sqlServerConnectionString,
            sqlServerOptions =>
            {
                sqlServerOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorNumbersToAdd: null);
            }))
    .AddDbContextFactory<RestaurantsDbContext>(options =>
        options.UseNpgsql(postgresConnectionString, options =>
        {
            options.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(5),
                errorCodesToAdd: null);
        }))
    .AddScoped<ISetupCompanyMigrationService, SetupCompanyMigrationService>()
    .AddScoped<IMenuMigrationService, MenuMigrationService>()
    .AddScoped<IBranchMasterMigrationService, BranchMasterMigrationService>()
    .AddScoped<ISetupMasterMigrationService, SetupMasterMigrationService>()
    .AddScoped<ISetupMasterDetailMigrationService, SetupMasterDetailMigrationService>()
    .AddScoped<IPaymentModeMigrationService, PaymentModeMigrationService>()
    .AddScoped<IGSTMigrationService, GSTMigrationService>()
    .AddScoped<IOrderModeCompanyMappingMigrationService, OrderModeCompanyMappingMigrationService>()
    .AddScoped<IDiscountMigrationService, DiscountMigrationService>()
    .AddScoped<IProductSizeMigrationService, ProductSizeMigrationService>()
    .AddScoped<IFlavourMigrationService, FlavourMigrationService>()
    .AddScoped<ICityMigrationService, CityMigrationService>()
    .AddScoped<IAreaMigrationService, AreaMigrationService>()
    .AddScoped<ISetupCompanySettingsMigrationService, SetupCompanySettingsMigrationService>()
    .AddScoped<ICustomerDataImportService, CustomerDataImportService>()
    .AddScoped<IUserLoginMigrationService, UserLoginMigrationService>()
    .AddScoped<IRidersMigrationService, RidersMigrationService>()
    .AddScoped<Implementation>();

var app = builder.Build();

// Optional: minimal endpoint (useful for health checks)
app.MapGet("/import/{companyId:int}", async (int companyId, [FromServices] ILogger<Program> logger, [FromServices] Implementation impl, [FromServices] IDbContextFactory<SqlServerDbContext> sqlServerDbContextFactory, HttpContext httpContext, [FromQuery] string selection = "") =>
{
    List<string> list = ["all", "setupCompany", "setupMaster", "setupMasterDetail", "city", "area", "branchMaster", "productSize", "flavour", "menu", "paymentMode", "setupCompanySettings", "discount", "customerData", "gst", "userLogin", "riders", "orderMode"];

    if (string.IsNullOrEmpty(selection) || !list.Contains(selection))
    {
        logger.LogWarning("Invalid selection provided: {Selection}. No migration will be performed.", selection);
        return Results.BadRequest($"Invalid selection: {selection}");
    }
    using var sqlServerDbContext = sqlServerDbContextFactory.CreateDbContext();
    var company = await sqlServerDbContext.SetupCompanies.FirstOrDefaultAsync(x => x.CompanyId == companyId, httpContext.RequestAborted);
    if (company == null)
    {
        logger.LogWarning("Company not found: {CompanyId}", companyId);
        return Results.NotFound("Company not found");
    }

    var url = company.WebsiteUrl;
    if (url == null)
    {
        logger.LogWarning("Company website URL not found for CompanyId: {CompanyId}", companyId);
        return Results.NotFound("Company website URL not found");
    }
    var domain = url
                .Replace("http://", "")
                .Replace("https://", "")
                .Replace("www.", "")
                .Split('/')[0];
    var response = await impl.Import(companyId, domain, selection, httpContext.RequestAborted);
    try
    {
        var httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromMinutes(5),
            BaseAddress = new Uri($"http://gatewayservice:8080")
        };

        var httpResponse = await httpClient.GetAsync($"clear?domain={domain}");
        if (httpResponse.IsSuccessStatusCode == false)
        {
            return Results.Ok($"Import completed but failed to clear cache\nStatusCode: {httpResponse.StatusCode}");
        }
    }
    catch (Exception ex)
    {
        return Results.Ok($"Import completed but failed to clear cache\n{ex.Message}");
    }
    return response;
});

app.MapGet("health", ([FromServices] ILogger<Program> logger, [FromServices] IDbContextFactory<SqlServerDbContext> sqlDbContextFactory, [FromServices] IDbContextFactory<RestaurantsDbContext> restaurantDbContextFactory) =>
{
    using var sqlServerDbContext = sqlDbContextFactory.CreateDbContext();
    if (sqlServerDbContext.Database.CanConnect() == false)
    {
        logger.LogError("SQL Server Database connection failed");
        return Results.Problem(statusCode: 503);
    }

    using var restaurantDbContext = restaurantDbContextFactory.CreateDbContext();
    if (restaurantDbContext.Database.CanConnect() == false)
    {
        logger.LogError("Postgres Database connection failed");
        return Results.Problem(statusCode: 503);
    }
    return Results.Ok("Service is healthy");
});

await app.RunAsync();
