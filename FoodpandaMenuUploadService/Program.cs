using FoodpandaMenuUploadService;
using FoodpandaMenuUploadService.Classes;
using FoodpandaMenuUploadService.Interfaces;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration
    .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services
.AddSingleton<IAccessToken, AccessToken>()
.AddSingleton<IFoodPandaTransformer, FoodPandaTransformer>()
.AddSingleton<IMenuService, MenuService>()
.AddSingleton<IUploadService, UploadService>();

builder.Services.AddGrpc();

var app = builder.Build();

//app.MapGet("/UploadMenu/{id}", async ([FromServices] IAccessToken accessTokenService, [FromServices] IFoodPandaTransformer foodPandaTransformer, [FromServices] IMenuService menuService, [FromServices] IUploadService uploadService, int id) =>
//{
//    var ygenJson = await menuService.GetRestaurantMenu(id);
//    if (ygenJson == null)
//        return Results.NotFound("Menu not found for the given restaurant ID.");
//    var pandaNode = foodPandaTransformer.Transform(ygenJson);
//    var response = await uploadService.Initiate(pandaNode);
//    return Results.Ok(response);
//});

app.MapGrpcService<FpUploadMenuServiceImpl>();

app.UseHttpsRedirection();

app.Run();
