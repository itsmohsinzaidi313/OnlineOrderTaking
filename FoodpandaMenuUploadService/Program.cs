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
    var pandaNode = TransformToFoodpanda(ygenJson);
    var pandaNodeString = pandaNode.ToString();
    var response = await SendToFoodpanda(pandaNode);
    return Results.Ok(response);

});
// Configure the HTTP request pipeline.

app.UseHttpsRedirection();



app.Run();
const string CallbackUrls = "https://ygensystems.com/api/v2/OnlineOrders/PosIntegration/BBECAFA9-48BA-46BE-A5CF-26E7B0ED76CA";
static async Task<JsonNode?> GetRestaurantMenu(int id)
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

static async Task<string> SendToFoodpanda(JsonNode manu)
{
    var options = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
    var content = JsonContent.Create(manu, options: options);
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

static async Task<string?> RequestAccessTokenAsync()
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

static JsonObject TransformToFoodpanda(JsonNode source)
{
    var items = new JsonObject();
    var categories = source["Categories"]?.AsArray() ?? [];
    var products = source["Products"]?.AsArray() ?? [];
    foreach (var p in products.Select(x => x?.AsObject()))
    {
        if (((bool?)p?["IsEnable"] ?? false) == false) continue;
        var dealItems = p?["DealItems"]?.AsArray() ?? [];
        if (dealItems.Count == 0)
        {
            if (p == null) continue;
            var pdList = p["ProductDetails"]?.AsArray() ?? [];
            foreach (var pd in pdList.Select(x => x?.AsObject()))
            {
                if (pd == null) continue;
                var fpItem = CreateProduct(p, pd);
            }
        }
    }
    return new JsonObject
    {
        ["callbackUrl"] = CallbackUrls,
        ["catalog"] = new JsonObject
        {
            ["items"] = items
        },
        ["vendors"] = new JsonArray("POS123")
    };
}


static JsonObject CreateProduct(JsonObject p, JsonObject pd)
{
    var id = (int)pd["ProductId"]!;
    var pdid = (int)p["ProductDetailId"]!;
    var objId = $"prd{id}{pdid}";
    var sizeName = (string?)pd["SizeName"] ?? string.Empty;
    var flavourName = (string?)pd["FlavourName"] ?? string.Empty;
    var prodName = (string)p["ProductName"]!;
    var name = prodName;

    if (sizeName != "-") name += $" {sizeName}";
    if (flavourName != "-") name += $" {flavourName}";


    var description = (string?)p["ProductDescription"] ?? string.Empty;
    var price = (double?)pd["Price"] ?? 0.00;
    var prod = new JsonObject
    {
        ["id"] = objId,
        ["type"] = "Product",
        ["title"] = new JsonObject { ["default"] = prodName },
        ["description"] = new JsonObject { ["default"] = description },
        ["active"] = true,
        ["isPrepackedItem"] = false,
        ["isExpressItem"] = false,
        ["excludeDishInformation"] = false,
        ["price"] = price
    };

    return prod;
}

static JsonObject CreatePricelessProduct(JsonObject p, JsonObject pd)
{
    var title = p["ProductName"];
    var obj = new JsonObject()
    {
        ["title"] = new JsonObject() { ["default"] = title},
        ["type"] = "Product",
        ["description"] = new JsonObject(),
        
    };

    return obj;
}