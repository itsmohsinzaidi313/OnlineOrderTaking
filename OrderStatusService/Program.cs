using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderStatusService;
using PointofSaleModels.PGDatabaseModels;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;

var builder = WebApplication.CreateBuilder(args);
const string PostgressConnectionString = "Host=haproxy;Port=5433;Database=restaurants;Username=postgres;Password=postgrespass";
var rabbitMqSection = builder.Configuration.GetSection("RABBITMQ");
// Add services to the container.
builder.Services.AddDbContextFactory<RestaurantsContext>(options =>
    options.UseNpgsql(PostgressConnectionString,
                    npgsqlOptions =>
                    {
                        npgsqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(5),
                            errorCodesToAdd: null);
                    }))
    .Configure<RabbitMqSettings>(rabbitMqSection)
    .AddSingleton<RabbitMqConnection>()
    .AddSingleton<IRabbitMqPublisher, RabbitMqPublisher>()
    .AddSingleton<IQueueAction, RequestQueueAction>()
    .AddHostedService<RequestQueueListener>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGet("/health", ([FromServices] RestaurantsContext context) =>
{
    if (!context.Database.CanConnect())
    {
        return Results.Problem("Cannot connect to the database.");
    }
    return Results.Ok();
});

app.Run();
