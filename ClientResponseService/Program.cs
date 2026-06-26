using ClientResponseService;
using ClientResponseService.Models;
using ClientResponseService.ServiceResponseListeners;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PointofSaleModels.HealthChecks;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;
using StackExchange.Redis;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);

// Bind settings
builder.Services
    .Configure<RabbitMqSettings>(builder.Configuration.GetSection("RABBITMQ"))
    .Configure<RedisSettings>(builder.Configuration.GetSection("REDIS"));

var redisSettings = builder.Configuration.GetSection("REDIS").Get<RedisSettings>() ?? new RedisSettings();

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

builder.Services.AddHealthChecks()
    .AddCheck<RedisHealth>("health_check");

var app = builder.Build();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapHub<GatewayHub>("/gatewayHub");
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
