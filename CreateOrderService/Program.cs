using CreateOrderService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PointofSaleModels.PGDatabaseModels;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;
using StackExchange.Redis;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureAppConfiguration((hostingContext, config) =>
{
    config.AddEnvironmentVariables();
})
.ConfigureServices((context, services) =>
{
    var dbConnectionString = context.Configuration.GetConnectionString("Default");

    var redisConnectionString = context.Configuration.GetConnectionString("Redis")
    ?? throw new InvalidOperationException("Redis connection string is not configured.");
    services
    .AddDbContext<PgDbContext>(
        options => options.UseNpgsql(
            dbConnectionString,
                npgsqlOptions =>
                {
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorCodesToAdd: null);
                }))
    .Configure<RabbitMqSettings>(context.Configuration.GetSection("RABBITMQ"))
    .AddSingleton<RabbitMqConnection>()
    .AddSingleton<Implementation>()
    .AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect(redisConnectionString))
    .AddSingleton<IRabbitMqPublisher, RabbitMqPublisher>()
    .AddSingleton<IQueueAction, RequestQueueAction>()
    .AddHostedService<RequestQueueListener>();
});

var host = builder.Build();
await host.RunAsync();
