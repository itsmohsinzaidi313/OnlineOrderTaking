using GatewayService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;
using StackExchange.Redis;
using GatewayService.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

// Bind settings
builder.Services
    .Configure<RabbitMqSettings>(builder.Configuration.GetSection("RABBITMQ"))
    .Configure<RedisSettings>(builder.Configuration.GetSection("REDIS"))
    .Configure<JwtSettings>(builder.Configuration.GetSection("JWT"));

var redisSettings = builder.Configuration.GetSection("REDIS").Get<RedisSettings>() ?? new RedisSettings();

// Services
builder.Services
    .AddSignalR()
    .AddStackExchangeRedis(redisSettings.ConnectionString, opts => opts.Configuration.ChannelPrefix = RedisChannel.Literal("GatewayService"));

builder.Services.AddCors(c => c.AddDefaultPolicy(p => p.AllowAnyHeader().AllowAnyMethod().SetIsOriginAllowed(_ => true).AllowCredentials()));

builder.Services
    .AddSingleton<RabbitMqConnection>()
    .AddSingleton<IRabbitMqPublisher, RabbitMqPublisher>()
    .AddSingleton<MenuServiceResponseAction>()
    .AddSingleton<IQueueAction>(context => context.GetRequiredService<MenuServiceResponseAction>())
    .AddHostedService<MenuServiceResponseListener>()
    .AddSingleton<IConnectionMultiplexer>(context =>
    {
        var configuration = ConfigurationOptions.Parse(redisSettings.ConnectionString, true);
        return ConnectionMultiplexer.Connect(configuration);
    })
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
        // options.Events = new JwtBearerEvents
        // {
        //     OnMessageReceived = ctx =>
        //     {
        //         var accessToken = ctx.Request.Query["access_token"].ToString();
        //         if (!string.IsNullOrEmpty(accessToken) && ctx.HttpContext.Request.Path.StartsWithSegments("/gatewayHub"))
        //             ctx.Token = accessToken;
        //         return Task.CompletedTask;
        //     }
        // };
    });

builder.Services.AddAuthorization();

var app = builder.Build();
app.UseRouting();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapHub<GatewayHub>("/gatewayHub");
app.MapControllers();

var configuredRedis = app.Configuration["REDIS:ConnectionString"] ?? app.Configuration["Redis:ConnectionString"];
if (!string.IsNullOrWhiteSpace(configuredRedis))
    app.Logger.LogInformation("SignalR backplane enabled via Redis at {RedisEndpoint}", configuredRedis);

app.Run();
