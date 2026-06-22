using FoodpandaIntegrationService;
using Microsoft.EntityFrameworkCore;
using PointofSaleModels.HealthChecks;
using PointofSaleModels.PGDatabaseModels;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;
using static PointofSaleModels.Protos.CreateOrderService;

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

var rabbitMqSection = builder.Configuration.GetSection("RABBITMQ") ?? throw new InvalidOperationException("RabbitMQ section is not configured.");

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
    .AddSingleton<IRabbitMqPublisher, RabbitMqPublisher>()
    .AddHostedService<RequestQueueListener>()
    .AddHealthChecks()
    .AddCheck<PostgresHealth>("health_check");
builder.Services.AddGrpcClient<CreateOrderServiceClient>(x =>
{
    var address = builder.Configuration["GRPC:CREATEORDERHOST"] ?? throw new InvalidOperationException("CreateOrderService gRPC host is not configured.");
    x.Address = new Uri(address);
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapHealthChecks("/health");
app.Run();
