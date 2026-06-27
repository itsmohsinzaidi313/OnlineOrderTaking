using PointofSaleModels.Services;
using PointofSaleModels.Settings;
using StackExchange.Redis;
using System.Text;
using static PointofSaleModels.Protos.PushNotificationService;
using static PointofSaleModels.Protos.OrderHistoryService;
using static PointofSaleModels.Protos.GeneralSeoDataService;
using PointofSaleModels.HealthChecks;
using static PointofSaleModels.Protos.CreateOrderService;
using ClientResponseService.ServiceResponseListeners;
using ClientResponseService;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);

// Bind settings
builder.Services
    .Configure<RabbitMqSettings>(builder.Configuration.GetSection("RABBITMQ"))
    .Configure<RedisSettings>(builder.Configuration.GetSection("REDIS"));

var redisSettings = builder.Configuration.GetSection("REDIS").Get<RedisSettings>() ?? new RedisSettings();

// Services
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services
    .AddSignalR()
    .AddStackExchangeRedis(redisSettings.ConnectionString, opts => opts.Configuration.ChannelPrefix = RedisChannel.Literal("GatewayService"));

builder.Services
    .AddSingleton<RabbitMqConnection>()
    .AddSingleton<IRabbitMqPublisher, RabbitMqPublisher>()
    .AddHostedService<CreateOrderServiceResponseListener>()
    .AddHostedService<SettingsDataServiceResponseListener>()
    .AddHostedService<MenuServiceResponseListener>()
    .AddHostedService<OrderHistoryServiceResponseListener>()
    .AddHostedService<ClientNotificationServiceResponseListener>()
    .AddHostedService<OrderUpdateServiceResponseListener>()
    .AddHostedService<CustomerOrderHistoryServiceResponseListener>()
    .AddSingleton<IConnectionMultiplexer>(context =>
    {
        var configuration = ConfigurationOptions.Parse(redisSettings.ConnectionString, true);
        return ConnectionMultiplexer.Connect(configuration);
    })
    .AddSingleton<Implementation>()
    .AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddAuthorization();

builder.Services.AddHealthChecks()
    .AddCheck<RedisHealth>("health_check");

var app = builder.Build();

app.UseRouting();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapHub<GatewayHub>("/gatewayHub");
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
