using FoodpandaMenuUploadService;
using FoodpandaMenuUploadService.Classes;
using FoodpandaMenuUploadService.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration
    .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Add services to the container.
var sqlServerConnectionString =
    builder.Configuration.GetConnectionString("SqlServer")
    ?? throw new InvalidOperationException("SqlServer connection string is not configured.");
builder.Services
.AddSingleton<IAccessToken, AccessToken>()
.AddSingleton<IFoodPandaTransformer, FoodPandaTransformer>()
.AddSingleton<IMenuService, MenuService>()
.AddSingleton<IUploadService, UploadService>();
builder.Services
    .AddDbContextFactory<SqlServerDbContext>(options =>
        options.UseSqlServer(
            sqlServerConnectionString,
            sqlServerOptions =>
            {
                sqlServerOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorNumbersToAdd: null);
            }));

var app = builder.Build();
app.MapGet("/UploadMenu/{id}", async ([FromServices] IAccessToken accessTokenService, [FromServices] IFoodPandaTransformer foodPandaTransformer, [FromServices] IMenuService menuService, [FromServices] IUploadService uploadService, int id) =>
{
    var ygenJson = await menuService.GetRestaurantMenu(id);
    if (ygenJson == null)
        return Results.NotFound("Menu not found for the given restaurant ID.");
    var pandaNode = foodPandaTransformer.Transform(ygenJson);
    var response = await uploadService.Initiate(pandaNode);
    return Results.Ok(response);
});

app.UseHttpsRedirection();

app.Run();
