using PointofSaleModels.Services;
using PointofSaleModels.Settings;
using PushNotificationService;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);

// Add services to the container.
builder.Services
    .Configure<VapidSettings>(builder.Configuration.GetSection("VAPID"))
    .Configure<RedisSettings>(builder.Configuration.GetSection("REDIS"))
    .Configure<RabbitMqSettings>(builder.Configuration.GetSection("RABBITMQ"));

builder.Services.AddSingleton<WebPushService>();
builder.Services.AddSingleton<IConnectionMultiplexer>(context =>
 {
     var connectionString =
         builder.Configuration.GetSection("REDIS").Get<RedisSettings>()?.ConnectionString ?? throw new InvalidOperationException("Redis connection string is not configured.");
     var configuration = ConfigurationOptions.Parse(connectionString, true);
     return ConnectionMultiplexer.Connect(configuration);
 });

builder.Services
    .AddSingleton<RabbitMqConnection>()
    .AddHostedService<RequestQueueListener>();
builder.Services.AddGrpc();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.Map("/health", () => Results.Ok());

app.MapGrpcService<PushNotificationServiceImpl>();
app.Run();
