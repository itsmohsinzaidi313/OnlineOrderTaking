using GatewayService;
// using GatewayService.Models; // already imported above
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;
using StackExchange.Redis;
using GatewayService.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection("Redis"));
// Bind Jwt settings from configuration (appsettings.json or environment variables)
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

// Ensure environment variables are used as a fallback when appsettings.json does not provide values.
builder.Services.PostConfigure<JwtSettings>(opts =>
{
    if (string.IsNullOrWhiteSpace(opts.Key))
        opts.Key = Environment.GetEnvironmentVariable("JWT__KEY") ?? opts.Key;
    if (string.IsNullOrWhiteSpace(opts.Issuer))
        opts.Issuer = Environment.GetEnvironmentVariable("JWT__ISSUER") ?? opts.Issuer;
    if (string.IsNullOrWhiteSpace(opts.Audience))
        opts.Audience = Environment.GetEnvironmentVariable("JWT__AUDIENCE") ?? opts.Audience;
    if (opts.ExpireMinutes == 0)
    {
        var env = Environment.GetEnvironmentVariable("JWT__EXPIREMINUTES");
        if (int.TryParse(env, out var m)) opts.ExpireMinutes = m;
    }
});

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
    .AddSingleton<IRabbitMqPublisher, RabbitMqPublisher>()
    // Register concrete action types so constructors that request them can be resolved
    .AddSingleton<MenuServiceResponseAction>()
    // Also keep the IQueueAction registrations (map to the concrete instances)
    .AddSingleton<IQueueAction>(sp => sp.GetRequiredService<MenuServiceResponseAction>())
    .AddHostedService<MenuServiceResponseListener>();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();

// Configure JWT Bearer authentication using the bound JwtSettings
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtSettings = jwtSection.Get<JwtSettings>() ?? new JwtSettings();
if (string.IsNullOrWhiteSpace(jwtSettings.Key) || string.IsNullOrWhiteSpace(jwtSettings.Issuer) || string.IsNullOrWhiteSpace(jwtSettings.Audience))
{
    // Do not throw here; token endpoint will validate presence and return an error if missing. Write a small startup note.
    Console.WriteLine("Jwt settings appear incomplete; ensure Jwt:Key, Jwt:Issuer and Jwt:Audience are configured if you want to generate tokens.");
}

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
        // Allow SignalR clients to send the JWT as an "access_token" query string
        // parameter (used by the JS SignalR client's accessTokenFactory on WebSockets).
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"].ToString();
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/gatewayHub"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

var app = builder.Build();

app.UseRouting();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<GatewayHub>("/gatewayHub");

// Controller endpoints are used for token generation and refresh.
app.MapControllers();

// Log whether SignalR Redis backplane is enabled
var configuredRedis = app.Configuration["Redis:ConnectionString"];
if (!string.IsNullOrWhiteSpace(configuredRedis))
{
    app.Logger.LogInformation("SignalR backplane enabled via Redis at {RedisEndpoint}", configuredRedis);
}

app.Run();
    