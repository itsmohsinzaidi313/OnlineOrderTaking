using GatewayService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;
using StackExchange.Redis;
using GatewayService.Models;
using System.Text;
using GatewayService.ServiceResponseListeners;
using static PointofSaleModels.Protos.PushNotificationService;
using static PointofSaleModels.Protos.OrderHistoryService;
using static PointofSaleModels.Protos.GeneralSeoDataService;
using PointofSaleModels.HealthChecks;
using static PointofSaleModels.Protos.CreateOrderService;

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
    //.AddHostedService<CreateOrderServiceResponseListener>()
    //.AddHostedService<SettingsDataServiceResponseListener>()
    //.AddHostedService<MenuServiceResponseListener>()
    //.AddHostedService<OrderHistoryServiceResponseListener>()
    //.AddHostedService<ClientNotificationServiceResponseListener>()
    //.AddHostedService<OrderUpdateServiceResponseListener>()
    //.AddHostedService<CustomerOrderHistoryServiceResponseListener>()
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

builder.Services.AddGrpcClient<PushNotificationServiceClient>(o =>
{
    var address = builder.Configuration["GRPC:PUSHNOTIFICAIONHOST"] ?? throw new InvalidOperationException("PushNotificationService gRPC host is not configured.");
    o.Address = new Uri(address);
    Console.WriteLine($"Configured PushNotificationService gRPC client with address: {o.Address}");
});

builder.Services.AddGrpcClient<OrderHistoryServiceClient>(x =>
{
    var address = builder.Configuration["GRPC:ORDERHISTORYHOST"] ?? throw new InvalidOperationException("OrderHistoryService gRPC host is not configured.");
    x.Address = new Uri(address);
    Console.WriteLine($"Configured OrderHistoryService gRPC client with address: {x.Address}");
});

builder.Services.AddGrpcClient<GeneralSeoDataServiceClient>(x =>
{
    var address = builder.Configuration["GRPC:GENERALSEODATAHOST"] ?? throw new InvalidOperationException("GeneralSeoDataService gRPC host is not configured.");
    x.Address = new Uri(address);
    Console.WriteLine($"Configured GeneralSeoDataService gRPC client with address: {x.Address}");
});

builder.Services.AddGrpcClient<CreateOrderServiceClient>(x =>
{
    var address = builder.Configuration["GRPC:CREATEORDERHOST"] ?? throw new InvalidOperationException("CreateOrderService gRPC host is not configured.");
    x.Address = new Uri(address);
});

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
