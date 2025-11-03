using GatewayService;
using GatewayService.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PointofSaleModels.Application;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;
using StackExchange.Redis;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime;
using System.Security.Claims;
using System.Text;

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
    // Also keep the IQueueAction registrations (map to the concrete instances)
    .AddSingleton<IQueueAction>(sp => sp.GetRequiredService<MenuServiceResponseAction>())
    .AddHostedService<MenuServiceResponseListener>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!))
        };
    });

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseRouting();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<GatewayHub>("/gatewayHub");

// Log whether SignalR Redis backplane is enabled
var configuredRedis = app.Configuration["Redis:ConnectionString"];
if (!string.IsNullOrWhiteSpace(configuredRedis))
{
    app.Logger.LogInformation("SignalR backplane enabled via Redis at {RedisEndpoint}", configuredRedis);
}

// Add the minimal API endpoint for generating JWT tokens
app.MapPost("/generate-token", (UserCredentials credentials, IConfiguration config) =>
{
    // Validate user credentials (hardcoded for simplicity)
    if (credentials.Username != "admin" || credentials.Password != "password")
    {
        return Results.Unauthorized();
    }
    var _settings = config.GetSection("Jwt").Get<JwtSettings>() ?? throw new Exception("JWT settings not configured");
    var userId = $"{Guid.NewGuid().ToString("N")[..4]}-{Guid.NewGuid().ToString("N")[..4]}";
    // Generate JWT token
    var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim("userId", userId)
        };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var expires = DateTime.UtcNow.AddMinutes(_settings.ExpireMinutes <= 0 ? 60 : _settings.ExpireMinutes);

    var token = new JwtSecurityToken(
        issuer: _settings.Issuer,
        audience: _settings.Audience,
        claims: claims,
        expires: expires,
        signingCredentials: creds
    );

    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

    return Results.Ok(new { Token = tokenString });
});

app.Run();