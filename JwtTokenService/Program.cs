using JwtTokenService.Services;
using PointofSaleModels.Settings;
using PointofSaleModels.Services;

var builder = WebApplication.CreateBuilder(args);

// Bind Jwt settings
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddSingleton<TokenService>();
builder.Services.AddSingleton<ConnectionRegistry>();

// Configure RabbitMQ and queue listener (no HTTP endpoints)
builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.AddSingleton<RabbitMqConnection>();
builder.Services.AddScoped<PointofSaleModels.Services.IQueueExecution, JwtTokenService.Services.QueueListener>();
builder.Services.AddHostedService<RabbitMqConsumerService>();

var app = builder.Build();

// Run the background worker that listens to RabbitMQ
await app.RunAsync();

public partial class Program { }
