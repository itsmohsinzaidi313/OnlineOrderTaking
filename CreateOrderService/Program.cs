using CreateOrderService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PointofSaleModels.DatabaseModels;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;

var builder = Host.CreateDefaultBuilder(args);
builder.ConfigureAppConfiguration((hostingContext, config) =>
{
    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
})
.ConfigureServices((hostingContext, services) =>
{
    services.AddDbContext<RestaurantErpWebContext>(options =>
    {
        services
        .AddDbContext<RestaurantErpWebContext>(
            options => options.UseNpgsql(
                hostingContext.Configuration.GetConnectionString("Default"))
        )
        .Configure<RabbitMqSettings>(hostingContext.Configuration.GetSection("RabbitMQ"))
    .AddSingleton<RabbitMqConnection>()
    .AddSingleton<IRabbitMqPublisher, RabbitMqPublisher>()
        .AddSingleton<IQueueAction, RequestQueueAction>()
        .AddHostedService<RequestQueueListener>();
    });
});
builder.Build();
