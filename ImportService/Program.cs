using ImportService;
using ImportService.Data;
using ImportService.Interfaces;
using ImportService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

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
    .AddScoped<ISetupCompanySettingsMigrationService, SetupCompanySettingsMigrationService>();

var app = builder.Build();

// Optional: minimal endpoint (useful for health checks)
app.MapGet("/import/{companyId:int}", async (int companyId, [FromServices] Implementation impl, HttpContext httpContext) =>
{
    return await impl.Import(companyId, httpContext.RequestAborted);
});
app.MapGet("health", (SqlServerDbContext sqlServerDbContext) => {
    if (sqlServerDbContext.Database.CanConnect() == false)
    {
        return Results.Problem("Sql Database connection failed", statusCode: 503);
    }
    return Results.Ok("Service is healthy");
});


await app.RunAsync();
