using Microsoft.EntityFrameworkCore;
using Microservice;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// Add API Explorer services for Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Products API",
        Version = "v1",
    });
});

// Add EF Core with PostgreSQL + resilient retries for transient failures (e.g., Patroni failover)
builder.Services.AddDbContext<ProductsDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsqlOptions =>
        {
            npgsqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(5),
                errorCodesToAdd: null);
        }));

// Add seeding service
builder.Services.AddScoped<DatabaseSeedingService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Products API V1");
        c.DocumentTitle = "Products API Documentation";
        c.DefaultModelsExpandDepth(-1); // Hide schemas section by default
        c.DisplayRequestDuration();
    });
}

// Database initialization - Apply migrations and seed data
// Create a scope when resolving scoped services from the application root
using (var scope = app.Services.CreateScope())
{
    await InitializeDatabaseAsync(scope.ServiceProvider);
}

app.UseRouting();
app.MapControllers();

app.Run();

static async Task InitializeDatabaseAsync(IServiceProvider services)
{
    // Resolve the scoped DatabaseSeedingService from the provided scope
    var seedingService = services.GetRequiredService<DatabaseSeedingService>();
    await seedingService.SeedAsync();
}


// rabbitmq >> redapple >> insert order