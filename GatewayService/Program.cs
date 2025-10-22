using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.StackExchangeRedis;
using StackExchange.Redis;
using RabbitMQ.Client;
using GatewayService.Services;
using GatewayService.Settings;
using GatewayService.Hubs;

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
builder.Services.AddHostedService<ConsumerService>();

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