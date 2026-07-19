using Microsoft.EntityFrameworkCore;
using PointofSaleModels.PGDatabaseModels;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration
    .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Add services to the container.
var dbConnectionString =
    builder.Configuration.GetConnectionString("POSTGRES")
    ?? throw new InvalidOperationException("Postgres connection string is not configured.");

builder.Services
    .AddDbContextFactory<RestaurantsContext>(
        options => options.UseNpgsql(
            dbConnectionString,
            npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorCodesToAdd: null);
            }));
var app = builder.Build();
// Configure the HTTP request pipeline.

app.UseHttpsRedirection();


app.Run();

