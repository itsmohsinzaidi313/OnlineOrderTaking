using GatewayService.Hubs;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection("Redis"));
// SignalR
var signalR = builder.Services.AddSignalR();
var redisConn = builder.Configuration.GetSection("Redis:ConnectionString").Value;
if (!string.IsNullOrWhiteSpace(redisConn))
{
    signalR.AddStackExchangeRedis(redisConn, options =>
    {
        options.Configuration.ChannelPrefix = RedisChannel.Literal("GatewayService");
    });
}
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .SetIsOriginAllowed(_ => true)
              .AllowCredentials();
    });
});
builder.Services.AddSingleton<RabbitMqConnection>();
builder.Services.AddHostedService<RabbitMqConsumerService>();

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