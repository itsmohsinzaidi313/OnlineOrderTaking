using GetMenuService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PointofSaleModels.DatabaseModels;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        services
        .AddDbContext<RestaurantErpWebContext>(
            options => options.UseNpgsql(
                context.Configuration.GetConnectionString("Default"))
        )
        .Configure<RabbitMqSettings>(context.Configuration.GetSection("RabbitMQ"))
        .AddSingleton<RabbitMqConnection>()
        .AddSingleton<IQueueAction, RequestQueueAction>()
        .AddHostedService<RequestQueueListener>();
    })
    .Build();

await host.RunAsync();