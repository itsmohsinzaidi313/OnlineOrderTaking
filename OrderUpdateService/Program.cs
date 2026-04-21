using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderUpdateService;
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
    .AddSingleton<IRabbitMqPublisher, RabbitMqPublisher>()
    .AddHostedService<RequestQueueListener>();
builder.Services.AddHealthChecks()
    .AddCheck<PostgresHealth>("health_check");

var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapHealthChecks("/health");

app.Run();
