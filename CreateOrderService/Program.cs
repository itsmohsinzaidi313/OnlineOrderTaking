using CreateOrderService;
using Microsoft.EntityFrameworkCore;
using PointofSaleModels.HealthChecks;
using PointofSaleModels.PGDatabaseModels;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Configuration
builder.Configuration
    .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Connection strings
var dbConnectionString =
    builder.Configuration.GetConnectionString("POSTGRES")
    ?? throw new InvalidOperationException("Postgres connection string is not configured.");

var rabbitMqSection = builder.Configuration.GetSection("RABBITMQ");

builder.Services
    .AddDbContextFactory<RestaurantsContext>(
        options => options.UseNpgsql(
            dbConnectionString,
            npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorCodesToAdd: null);
            }))
    .Configure<RabbitMqSettings>(rabbitMqSection)
    .AddSingleton<RabbitMqConnection>()
    .AddSingleton<Implementation>()
    .AddSingleton<IRabbitMqPublisher, RabbitMqPublisher>()
    .AddHostedService<RequestQueueListener>()
    .AddHealthChecks()
    .AddCheck<PostgresHealth>("health_check");

builder.Services.AddGrpc();
builder.WebHost.ConfigureKestrel(options =>
{
    // gRPC endpoint on port 8080
    options.ListenAnyIP(8080, o =>
    {
        o.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
    });

    // Health check endpoint on port 8081
    options.ListenAnyIP(8081, o =>
    {
        o.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1;
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapHealthChecks("/health");

app.MapGrpcService<CreateOrderServiceImpl>();
app.Run();
