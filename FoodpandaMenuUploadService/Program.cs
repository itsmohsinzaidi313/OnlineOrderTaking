using FoodpandaMenuUploadService;
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

builder.Services.AddGrpc();
var app = builder.Build();
// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.MapGrpcService<FpUploadMenuServiceImpl>();


app.Run();

