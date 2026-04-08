using ExportService;
using ExportService.DatabaseContexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
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
var rabbitMqSection = builder.Configuration.GetSection("RABBITMQ") ?? throw new InvalidOperationException("RABBITMQ section is not configured.");

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
    .Configure<RabbitMqSettings>(rabbitMqSection)
    .AddSingleton<RabbitMqConnection>()
    .AddSingleton<OrderExportService>()
    .AddHostedService<RequestQueueListener>()
    .Configure<HostOptions>(options =>
    {
        options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
    });

var app = builder.Build();
app.MapGet("/health", ([FromServices] SqlServerDbContext sqlServerDb, [FromServices] RestaurantsDbContext restaurantsDb) =>
{
    if (!sqlServerDb.Database.CanConnect())
    {
        return Results.Problem("Cannot connect to SQL Server database", statusCode: 500);
    }
    if (!restaurantsDb.Database.CanConnect())
    {
        return Results.Problem("Cannot connect to PostgreSQL database", statusCode: 500);
    }
    return Results.Ok();
});

app.Run();
