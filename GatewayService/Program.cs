using GatewayService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;
using StackExchange.Redis;
using GatewayService.Models;
using System.Text;
using GatewayService.ServiceResponseListeners;
using GatewayService.Interfaces;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);

// Bind settings
builder.Services
    .Configure<RabbitMqSettings>(builder.Configuration.GetSection("RABBITMQ"))
    .Configure<RedisSettings>(builder.Configuration.GetSection("REDIS"))
    .Configure<JwtSettings>(builder.Configuration.GetSection("JWT"));

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
    .AddSingleton<DataServiceResponseAction>()
    .AddSingleton<IDataServiceResponseAction>(context => context.GetRequiredService<DataServiceResponseAction>())
    .AddHostedService<DataServiceResponseListener>()
    .AddSingleton<CreateOrderServiceResponseAction>()
    .AddSingleton<ICreateOrderResponseAction>(context => context.GetRequiredService<CreateOrderServiceResponseAction>())
    .AddHostedService<CreateOrderServiceResponseListener>()
    .AddSingleton<OrderStatusServiceResponseAction>()
    .AddSingleton<IOrderStatusResponseAction>(context => context.GetRequiredService<OrderStatusServiceResponseAction>())
    .AddHostedService<OrderStatusServiceResponseListener>()
    .AddSingleton<OrderNotificationServiceResponseAction>()
    .AddSingleton<IOrderNotificationResponseAction>(context => context.GetRequiredService<OrderNotificationServiceResponseAction>())
    .AddSingleton<SettingsDataServiceResponseAction>()
    .AddSingleton<ISettingsDataResponseAction>(context => context.GetRequiredService<SettingsDataServiceResponseAction>())
    .AddHostedService<SettingsDataServiceResponseListener>()
    .AddSingleton<MenuServiceResponseAction>()
    .AddSingleton<IMenuResponseAction>(context => context.GetRequiredService<MenuServiceResponseAction>())
    .AddHostedService<MenuServiceResponseListener>()
    .AddSingleton<OrderHistoryServiceResponseAction>()
    .AddSingleton<IOrderHistoryResponseAction>(context => context.GetRequiredService<OrderHistoryServiceResponseAction>())
    .AddHostedService<OrderHistoryServiceResponseListener>()
    .AddHostedService<OrderNotificationServiceResponseListener>()
    .AddSingleton<IConnectionMultiplexer>(context =>
    {
        var configuration = ConfigurationOptions.Parse(redisSettings.ConnectionString, true);
        return ConnectionMultiplexer.Connect(configuration);
    })
    .AddSingleton<Implementation>()
    .AddControllers();

builder.Services.AddEndpointsApiExplorer();

// JWT
var jwtSettings = builder.Configuration.GetSection("JWT").Get<JwtSettings>() ?? new JwtSettings();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        if (!string.IsNullOrWhiteSpace(jwtSettings.Key))
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
                ValidateIssuer = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtSettings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        }

        // Allow SignalR to receive token via "access_token" query for websocket transport
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = ctx =>
            {
                var accessToken = ctx.Request.Query["access_token"].ToString();
                if (!string.IsNullOrEmpty(accessToken) && ctx.HttpContext.Request.Path.StartsWithSegments("/gatewayHub"))
                    ctx.Token = accessToken;
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();
app.UseRouting();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapHub<GatewayHub>("/gatewayHub");
app.MapControllers();

var configuredRedis = app.Configuration["REDIS:ConnectionString"] ?? app.Configuration["Redis:ConnectionString"];
if (!string.IsNullOrWhiteSpace(configuredRedis))
    app.Logger.LogInformation("SignalR backplane enabled via Redis at {RedisEndpoint}", configuredRedis);

app.Run();
