using GatewayService;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection("Redis"));
var signalR = builder.Services.AddSignalR();
var redisConn = builder.Configuration.GetSection("Redis:ConnectionString").Value ?? throw new InvalidOperationException("Redis connection string not configured");
signalR.AddStackExchangeRedis(redisConn, options =>
{
    options.Configuration.ChannelPrefix = RedisChannel.Literal("GatewayService");
});
builder.Services
    .AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .SetIsOriginAllowed(_ => true)
              .AllowCredentials();
    });
})
    .AddSingleton<RabbitMqConnection>()
    // Register concrete action types so constructors that request them can be resolved
    .AddSingleton<MenuServiceResponseAction>()
    .AddSingleton<JwtServiceResponseAction>()
    // Also keep the IQueueAction registrations (map to the concrete instances)
    .AddSingleton<IQueueAction>(sp => sp.GetRequiredService<MenuServiceResponseAction>())
    .AddSingleton<IQueueAction>(sp => sp.GetRequiredService<JwtServiceResponseAction>())
    .AddHostedService<MenuServiceResponseListener>()
    .AddHostedService<JwtServiceResponseListener>();

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseRouting();
app.UseCors();

app.MapHub<GatewayHub>("/gatewayHub");

// Log whether SignalR Redis backplane is enabled
var configuredRedis = app.Configuration["Redis:ConnectionString"];
if (!string.IsNullOrWhiteSpace(configuredRedis))
{
    app.Logger.LogInformation("SignalR backplane enabled via Redis at {RedisEndpoint}", configuredRedis);
}

app.Run();