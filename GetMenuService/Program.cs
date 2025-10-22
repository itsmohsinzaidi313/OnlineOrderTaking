using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using GetMenuService.Settings;
using Microsoft.Extensions.Configuration;
using GetMenuService.Services;
using PointofSaleModels.DatabaseModels;
using Microsoft.EntityFrameworkCore;

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
        .AddHostedService<ConsumerService>();
    })
    .Build();

await host.RunAsync();