using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PointofSaleModels.PGDatabaseModels;

var builder = WebApplication.CreateBuilder(args);
const string PostgressConnectionString = "Host=haproxy;Port=5433;Database=restaurants;Username=postgres;Password=postgrespass";
// Add services to the container.
builder.Services.AddDbContextFactory<RestaurantsContext>(options =>
    options.UseNpgsql(PostgressConnectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGet("/health", ([FromServices] RestaurantsContext context) =>
{
    if(!context.Database.CanConnect())   
    {
        return Results.Problem("Cannot connect to the database.");
    }
    return Results.Ok();
});

app.Run();
