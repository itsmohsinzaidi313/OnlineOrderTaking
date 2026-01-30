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
    .AddEnvironmentVariables()
    ;

// Connection strings
var sqlServerConnectionString =
    builder.Configuration.GetConnectionString("SqlServer")
    ?? throw new InvalidOperationException("SqlServer connection string is not configured.");
const string PostgressConnectionString = "Host=haproxy;Port=5433;Database=restaurants;Username=postgres;Password=postgrespass";
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
        options.UseNpgsql(PostgressConnectionString, options =>
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
    .AddScoped<Implementation>();

var app = builder.Build();

// Optional: minimal endpoint (useful for health checks)
app.MapGet("/import/{companyId:int}", async (int companyId, [FromServices] Implementation impl, [FromServices] SqlServerDbContext sqlServerDbContext, HttpContext httpContext) =>
{
    var company = await sqlServerDbContext.SetupCompanies.FirstOrDefaultAsync(x => x.CompanyId == companyId, httpContext.RequestAborted);
    if (company == null)
    {
        return Results.NotFound("Company not found");
    }

    var url = company.WebsiteUrl;
    if (url == null)
    {
        return Results.NotFound("Company website URL not found");
    }
    var domain = url
                .Replace("http://", "")
                .Replace("https://", "")
                .Replace("www.", "")
                .Split('/')[0];
    var dbName = domain.Split('.')[0];
    var response = await impl.Import(companyId, dbName, httpContext.RequestAborted);
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

app.MapGet("health", ([FromServices] SqlServerDbContext sqlServerDbContext) =>
{
    if (sqlServerDbContext.Database.CanConnect() == false)
    {
        return Results.Problem("Sql Database connection failed", statusCode: 503);
    }
    return Results.Ok("Service is healthy");
});


await app.RunAsync();
