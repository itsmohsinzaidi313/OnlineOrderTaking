using FoodpandaMenuUploadService;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration
    .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Add services to the container.
var sqlServerConnectionString =
    builder.Configuration.GetConnectionString("SqlServer")
    ?? throw new InvalidOperationException("SqlServer connection string is not configured.");

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
app.MapGet("/UploadMenu/{id}", async (int id) =>
{
    var ygenJson = await GetRestaurantMenu(id);
    if (ygenJson == null)
    {
        return Results.NotFound("Menu not found for the given restaurant ID.");
    }
    var menuNodeString = ygenJson.ToString();
    await File.WriteAllTextAsync("menu.json", menuNodeString);
    var pandaNode = TransformToFoodpanda(ygenJson);
    var pandaNodeString = pandaNode.ToString();
    await File.WriteAllTextAsync("body.json", pandaNodeString);
    var response = await SendToFoodpanda(pandaNode);
    return Results.Ok(response);

});
// Configure the HTTP request pipeline.

app.UseHttpsRedirection();



app.Run();
const string CallbackUrl = "https://ygensystems.com/api/v2/OnlineOrders/PosIntegration/BBECAFA9-48BA-46BE-A5CF-26E7B0ED76CA";

async Task<JsonNode?> GetRestaurantMenu(int id)
{
    var httpClient = new HttpClient();
    var body = new
    {
        OperationId = 1,
        CompanyId = id,
        OrderSourceValue = "WEB"
    };
    var content = JsonContent.Create(body);
    var response = await httpClient.PostAsync("http://85.190.242.39:5019/ExternalMenu", content);
    var ygenJson = await response.Content.ReadAsStringAsync();
    return JsonNode.Parse(ygenJson);
}

async Task<string> SendToFoodpanda(JsonNode menu)
{
    var options = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
    // var injection = JsonNode.Parse(GetJson());
    var content = JsonContent.Create(menu, options: options);
    var httpClient = new HttpClient();
    var accessToken = await RequestAccessTokenAsync();
    if (!string.IsNullOrEmpty(accessToken))
    {
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
    }
    var response = await httpClient.PutAsync($"https://integration-middleware.as.restaurant-partners.com/v2/chains/Ygen_PK_UAT/catalog", content);
    var responseContent = await response.Content.ReadAsStringAsync();
    return responseContent;
}

async Task<string?> RequestAccessTokenAsync()
{
    var token = File.Exists("access_token.txt") ? File.ReadAllText("access_token.txt") : null;
    if (!string.IsNullOrWhiteSpace(token))
    {
        return token;
    }
    const string baseUrl = "https://integration-middleware.as.restaurant-partners.com";
    const string loginPath = "/v2/login";
    const string username = "as-plugin-y-generation-systems-005";
    const string password = "KQ1D8Wcm0M";
    const string secret = "SnyteunCeerhicJofI";

    var apiUrl = $"{baseUrl.TrimEnd('/')}/{loginPath.TrimStart('/')}";
    var form = new Dictionary<string, string>
    {
        ["username"] = username,
        ["password"] = password,
        ["grant_type"] = "client_credentials"
    };

    if (!string.IsNullOrWhiteSpace(secret))
    {
        form["secret"] = secret;
    }

    using var content = new FormUrlEncodedContent(form);

    using var client = new HttpClient();
    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

    var loginResponse = await client.PostAsync(apiUrl, content);
    var loginContent = await loginResponse.Content.ReadAsStringAsync();

    Console.WriteLine($"POST {apiUrl} -> {(int)loginResponse.StatusCode} {loginResponse.ReasonPhrase}");
    if (!string.IsNullOrWhiteSpace(loginContent)) Console.WriteLine(loginContent);

    if (!loginResponse.IsSuccessStatusCode)
    {
        return null;
    }

    using var doc = System.Text.Json.JsonDocument.Parse(loginContent);
    if (doc.RootElement.TryGetProperty("access_token", out var accessTokenElement))
    {
        File.WriteAllText("access_token.txt", accessTokenElement.GetString() ?? string.Empty);
        return accessTokenElement.GetString();
    }

    return null;
}

JsonObject TransformToFoodpanda(JsonNode source)
{
    var data = (source?["Data"]?.AsObject()) ?? throw new Exception("");
    string[] WeekDays = ["MONDAY", "TUESDAY", "WEDNESDAY", "THURSDAY", "FRIDAY", "SATURDAY", "SUNDAY"];
    var items = new JsonObject();
    var categories = data["Categories"]?.AsArray() ?? [];
    var products = data["Products"]?.AsArray() ?? [];
    var fpcategories = categories.Select(x => new Category(
            Id: $"cat{x["CategoryId"]}",
            Title: $"{x["CategoryName"]}"
        ));
    var fpProducts = new List<Product>();
    var fpToppings = new List<Topping>();
    var fpToppingProducts = new List<Product>();
    var menuProducts = new List<Product>();
    foreach (var p in products)
    {
        foreach (var pd in p?["ProductDetails"]?.AsArray() ?? [])
        {
            var title = $"{p["ProductName"]}";
            if (pd["SizeName"]?.ToString() != "-") title += $" {pd["SizeName"]}";
            if (pd["FlavourName"]?.ToString() != "-") title += $" {pd["FlavourName"]}";
            var fpTempToppings = new List<Topping>();
            foreach (var di in pd?["DealItems"]?.AsArray() ?? [])
            {
                var fpTempToppingProducts = new List<Product>();
                foreach (var dd in di?["DealDescriptions"]?.AsArray() ?? [])
                {
                    var fpToppingProduct = new Product(
                    Id: $"prd{dd["DealProductDetailId"]}",
                    Title: title,
                    Description: (string?)p["ProductDescription"] ?? string.Empty,
                    Price: ((double?)pd["Price"])?.ToString("F2", CultureInfo.InvariantCulture)
                    );
                    fpTempToppingProducts.Add(fpToppingProduct);
                    fpToppingProducts.Add(fpToppingProduct);
                }
                var max = di["MinQuantity"]!.GetValue<int>();
                var min = di["MaxQuantity"]!.GetValue<int>();
                if (max == 0 && max == min)
                {
                    max = fpTempToppingProducts.Count;
                }
                var fpTopping = new Topping(
                            Id: $"prd{di["DealItemId"]}",
                            Title: $"{di["DealOptionName"]}",
                            Maximum: max,
                            Minimum: min,
                            Products: fpTempToppingProducts
                        );
                var tJson = Topping(fpTopping);
                items[fpTopping.Id] = tJson;
                fpTempToppings.Add(fpTopping);
                fpToppings.Add(fpTopping);
            }
            List<Image>? images = null;
            if (!string.IsNullOrEmpty(p?["ProductImage"]?.GetValue<string?>()))
            {
                var image = new Image(
                    Id: $"img{pd!["ProductDetailId"]}",
                    Url: $"https://services.eatx.pk{p?["ProductImage"]}"
                );
                images = [];
                images.Add(image);

                items[$"img{pd!["ProductDetailId"]}"] = Image(image);
            }
            var fpProduct = new Product(
                    Id: $"prd{pd!["ProductDetailId"]}",
                    Title: title,
                    Description: (string?)p["ProductDescription"] ?? string.Empty,
                    Price: ((double?)pd["Price"])?.ToString("F2", CultureInfo.InvariantCulture),
                    Toppings: fpTempToppings,
                    CategoryId: p!["ProductCategoryId"]!.GetValue<int>(),
                    Images: images
                );
            var pJson = Product(fpProduct);
            items[fpProduct.Id] = pJson;
            fpProducts.Add(fpProduct);
            if (((double?)pd["Price"] ?? 0) > 0)
            {
                menuProducts.Add(fpProduct);
            }
        }
    }

    foreach (var c in categories)
    {
        var list = new List<Product>();
        foreach (var p in fpProducts.Where(x => x.CategoryId == c!["CategoryId"]!.GetValue<int>()))
        {
            list.Add(p);
        }
        if (list.Count >= 1)
        {
            var category = new Category(
                Id: $"cat{c!["CategoryId"]}",
                Title: $"{c["CategoryName"]}",
                Products: list
            );
            items[category.Id] = Category(category);
        }
    }



    var schedule = new Schedule(
        Id: "sch1",
        StartTime: "00:00",
        EndTime: "23:59",
        Days: WeekDays
    );
    items[schedule.Id] = Schedule(schedule);
    var fpDeliveryMenuObj = new Menu(
        Id: "Delivery_Menu",
        Title: "Regular Menu",
        MenuType: "DELIVERY"
    );
    var fpDeliveryMenu = Menu(fpDeliveryMenuObj, schedules: [schedule], products: menuProducts
    );
    var fpPickupMenuObj = new Menu(
        Id: "PickUp_Menu",
        Title: "Regular Menu",
        MenuType: "PICKUP"
    );
    var fpPickUpMenu = Menu(fpPickupMenuObj, schedules: [schedule], products: menuProducts
    );
    items[fpDeliveryMenuObj.Id] = fpDeliveryMenu;
    items[fpPickupMenuObj.Id] = fpPickUpMenu;
    return new JsonObject
    {
        ["callbackUrl"] = CallbackUrl,
        ["catalog"] = new JsonObject
        {
            ["items"] = items
        },
        ["vendors"] = new JsonArray("POS123")
    };
}


JsonObject Product(Product product)
{
    string title = product.Title.Replace("\"", "").Replace("&", "");
    var obj = new JsonObject
    {
        ["id"] = product.Id,
        ["type"] = product.Type,
        ["title"] = new JsonObject { ["default"] = title },
    };
    if (!string.IsNullOrEmpty(product.Description) && product.Description != "null")
    {
        obj["description"] = new JsonObject { ["default"] = product.Description };
    }
    if (product.Price is not null)
    {
        obj["price"] = product.Price;
    }
    var variants = new JsonObject();
    foreach (var item in product.Variations ?? [])
    {
        variants[item.Id] = new JsonObject
        {
            ["id"] = item.Id,
            ["type"] = item.Type,
        };
    }
    if (variants.Count > 0)
        obj["variants"] = variants;

    var toppingObj = new JsonObject();
    foreach (var item in product.Toppings ?? [])
    {
        toppingObj[item.Id] = new JsonObject
        {
            ["id"] = item.Id,
            ["type"] = item.Type,
        };
    }
    if (toppingObj.Count > 0)
        obj["toppings"] = toppingObj;
    var images = new JsonObject();
    foreach (var img in product.Images ?? [])
    {
        images[img.Id] = new JsonObject
        {
            ["id"] = img.Id,
            ["type"] = img.Type
        };
    }
    if (images.Count > 0)
        obj["images"] = images;
    return obj;
}

JsonObject Topping(Topping topping)
{
    var obj = new JsonObject
    {
        ["id"] = topping.Id,
        ["type"] = topping.Type,
        ["title"] = new JsonObject { ["default"] = topping.Title },
        ["quantity"] = new JsonObject { ["maximum"] = topping.Maximum, ["minimum"] = topping.Minimum },
    };
    var productsObj = new JsonObject();

    foreach (var item in topping.Products ?? [])
    {
        productsObj[item.Id] = new JsonObject
        {
            ["id"] = item.Id,
            ["type"] = item.Type,
        };
    }
    if (productsObj.Count > 0)
        obj["products"] = productsObj;

    var images = new JsonObject();
    foreach (var img in topping.Images ?? [])
    {
        images[img.Id] = new JsonObject
        {
            ["id"] = img.Id,
            ["type"] = img.Type
        };
    }
    if (images.Count > 0)
        obj["images"] = images;
    return obj;
}


JsonObject Category(Category category, IEnumerable<Product>? products = null)
{
    string title = category.Title.Replace("\"", "").Replace("&", "and");
    var obj = new JsonObject
    {
        ["id"] = category.Id,
        ["type"] = category.Type,
        ["title"] = new JsonObject { ["default"] = title },
    };
    var productsObj = new JsonObject();
    foreach (var item in category.Products ?? products ?? [])
    {
        productsObj[item.Id] = new JsonObject
        {
            ["id"] = item.Id,
            ["type"] = item.Type,
        };
    }
    if (productsObj.Count > 0)
        obj["products"] = productsObj;
    return obj;
}

JsonObject Menu(Menu menu, IEnumerable<Schedule> schedules = null, IEnumerable<Product>? products = null)
{
    var obj = new JsonObject
    {
        ["id"] = menu.Id,
        ["type"] = menu.Type,
        ["title"] = new JsonObject { ["default"] = menu.Title },
        ["menuType"] = menu.MenuType
    };
    var schedulesObj = new JsonObject();
    foreach (var item in schedules ?? [])
    {
        schedulesObj[item.Id] = new JsonObject
        {
            ["id"] = item.Id,
            ["type"] = item.Type
        };
    }
    if (schedulesObj.Count > 0)
        obj["schedule"] = schedulesObj;

    var productsObj = new JsonObject();
    foreach (var item in products ?? [])
    {
        productsObj[item.Id] = new JsonObject
        {
            ["id"] = item.Id,
            ["type"] = item.Type,
        };
    }
    if (productsObj.Count > 0)
        obj["products"] = productsObj;
    return obj;
}

JsonObject Schedule(Schedule schedule)
{
    var weekDaysArray = new JsonArray();
    foreach (var day in schedule.Days)
    {
        weekDaysArray.Add(day);
    }
    return new JsonObject
    {
        ["id"] = schedule.Id,
        ["type"] = schedule.Type,
        ["startTime"] = schedule.StartTime,
        ["endTime"] = schedule.EndTime,
        ["weekDays"] = weekDaysArray
    };
}

JsonObject Image(Image image)
{
    return new JsonObject
    {
        ["id"] = image.Id,
        ["url"] = image.Url,
        ["type"] = image.Type,
    };
}

record Parent(string Id, string Type);
record Product(string Id, string Title, IEnumerable<Image>? Images = null, int? Order = null, string? Description = null, string? Price = null, List<Product>? Variations = null, List<Topping>? Toppings = null, string Type = "Product", int? CategoryId = null, string? InitPdId = null);
record Topping(string Id, string Title, int Minimum, int Maximum, IEnumerable<Image>? Images = null, IEnumerable<Product>? Products = null, string? Description = null, string Type = "Topping");
record Schedule(string Id, string StartTime, string EndTime, IEnumerable<string> Days, string Type = "ScheduleEntry");
record Menu(string Id, string Title, string MenuType, string Type = "Menu");
record Category(string Id, string Title, IEnumerable<Product>? Products = null, string Type = "Category");
record Image(string Id, string Url, string Type = "Image");