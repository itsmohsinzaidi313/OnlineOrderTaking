using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderHistoryService;
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
    builder.Configuration.GetConnectionString("Postgres")
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
    .AddScoped<IConnectionStringResolver, ConnectionStringResolver>();

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

app.MapGet("/health", async ([FromServices] IDbContextFactory<RestaurantsContext> contextFactory) =>
{
    var restaurantsContext = await contextFactory.CreateDbContextAsync();
    if (!await restaurantsContext.Database.CanConnectAsync())
    {
        return Results.Problem("Cannot connect to restaurants database.");
    }
    return Results.Ok();
});
app.MapGrpcService<OrderHistoryServiceImpl>();
app.Run();
