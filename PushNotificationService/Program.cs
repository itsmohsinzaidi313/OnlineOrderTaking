using PointofSaleModels.Settings;
using PushNotificationService;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);

// Add services to the container.
builder.Services.Configure<VapidSettings>(builder.Configuration.GetSection("VAPID"));
builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection("REDIS"));
builder.Services.AddScoped<WebPushService>();
builder.Services.AddSingleton<IConnectionMultiplexer>(context =>
 {
     var connectionString =
         builder.Configuration.GetSection("REDIS").Get<RedisSettings>()?.ConnectionString ?? throw new InvalidOperationException("Redis connection string is not configured.");
     var configuration = ConfigurationOptions.Parse(connectionString, true);
     return ConnectionMultiplexer.Connect(configuration);
 });

builder.Services.AddGrpc();
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5005, o =>
    {
        o.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.Map("/health", () => Results.Ok());

app.MapGrpcService<PushNotificationServiceImpl>();
app.Run();
