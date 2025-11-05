using GetMenuService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PointofSaleModels.DatabaseModels;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;
using StackExchange.Redis;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.Sources.Clear();
        config.AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        var dbConnectionString = context.Configuration.GetConnectionString("Default");

        services
        .AddDbContext<RestaurantErpWebContext>(
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
        .AddSingleton<IRabbitMqPublisher, RabbitMqPublisher>()
        .AddSingleton<IQueueAction, RequestQueueAction>()
        .AddHostedService<RequestQueueListener>();
    })
    .Build();

await host.RunAsync();