using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using GetMenuService.Settings;
using Microsoft.Extensions.Configuration;
using GetMenuService.Services;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        services.Configure<RabbitMqSettings>(context.Configuration.GetSection("RabbitMQ"));

        services.AddSingleton<RabbitMqConnection>();
        services.AddHostedService<ConsumerService>();
    })
    .Build();

await host.RunAsync();