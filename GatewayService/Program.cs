using GatewayService;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;
using StackExchange.Redis;
using System;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection("Redis"));

// Ensure environment variables are used as a fallback when appsettings.json does not provide values.
builder.Services.PostConfigure<RabbitMqSettings>(opts =>
{
    if (string.IsNullOrWhiteSpace(opts.HostName))
        opts.HostName = Environment.GetEnvironmentVariable("RABBITMQ__HOSTNAME") ?? opts.HostName;
    if (opts.Port == 0)
    {
        var portEnv = Environment.GetEnvironmentVariable("RABBITMQ__PORT");
        if (int.TryParse(portEnv, out var p)) opts.Port = p;
    }
    if (string.IsNullOrWhiteSpace(opts.UserName))
        opts.UserName = Environment.GetEnvironmentVariable("RABBITMQ__USERNAME") ?? opts.UserName;
    if (string.IsNullOrWhiteSpace(opts.Password))
        opts.Password = Environment.GetEnvironmentVariable("RABBITMQ__PASSWORD") ?? opts.Password;
});

builder.Services.PostConfigure<RedisSettings>(opts =>
{
    if (string.IsNullOrWhiteSpace(opts.ConnectionString))
        opts.ConnectionString = Environment.GetEnvironmentVariable("REDIS__CONNECTIONSTRING") ?? opts.ConnectionString;
});
var signalR = builder.Services.AddSignalR();
// Prefer appsettings value, fallback to environment variable REDIS__CONNECTIONSTRING
var redisConn = builder.Configuration["Redis:ConnectionString"] ?? Environment.GetEnvironmentVariable("REDIS__CONNECTIONSTRING");
if (string.IsNullOrWhiteSpace(redisConn))
    throw new InvalidOperationException("Redis connection string not configured (check appsettings.json or REDIS__CONNECTIONSTRING env var)");
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