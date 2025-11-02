using JwtTokenService.Services;
using PointofSaleModels.Settings;
using PointofSaleModels.Services;

var builder = WebApplication.CreateBuilder(args);

// Bind Jwt settings
builder.Services
.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"))
.AddSingleton<TokenService>()
.AddSingleton<ConnectionRegistry>()
.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMQ"))
.AddSingleton<RabbitMqConnection>()
.AddSingleton<IQueueAction, JwtRequestAction>()
.AddHostedService<JwtTokenRequestListener>();

var app = builder.Build();

await app.RunAsync();
