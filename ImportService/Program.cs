using ImportService;
using ImportService.Data;
using ImportService.Interfaces;
using ImportService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PointofSaleModels.PGDatabaseModels;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);
        // config.AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        var postgresConnectionString = context.Configuration.GetConnectionString("Postgres")
            ?? throw new InvalidOperationException("Postgres connection string is not configured.");
        var sqlServerConnectionString = context.Configuration.GetConnectionString("SqlServer")
            ?? throw new InvalidOperationException("SqlServer connection string is not configured.");

        services
        .AddDbContextFactory<PostgresDbContext>(
            options => options.UseNpgsql(
                postgresConnectionString,
                    npgsqlOptions =>
                    {
                        npgsqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(5),
                            errorCodesToAdd: null);
                    }))
        .AddDbContextFactory<SqlServerDbContext>(
            options => options.UseSqlServer(
                sqlServerConnectionString,
                    sqlServerOptions =>
                    {
                        sqlServerOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(5),
                            errorNumbersToAdd: null);
                    }))
        .Configure<RabbitMqSettings>(context.Configuration.GetSection("RABBITMQ"))
        .AddSingleton<RabbitMqConnection>()
        .AddSingleton<IRabbitMqPublisher, RabbitMqPublisher>()
        .AddSingleton<IQueueAction, RequestQueueAction>()
        .AddHostedService<RequestQueueListener>()
        .AddScoped<ISetupCompanyMigrationService, SetupCompanyMigrationService>()
        .AddScoped<IMenuMigrationService, MenuMigrationService>()
        .AddScoped<IBranchMasterMigrationService, BranchMasterMigrationService>()
        .AddScoped<ISetupMasterMigrationService, SetupMasterMigrationService>()
        .AddScoped<ISetupMasterDetailMigrationService, SetupMasterDetailMigrationService>()
        .AddScoped<IPaymentModeMigrationService, PaymentModeMigrationService>()
        .AddScoped<IGSTMigrationService, GSTMigrationService>()
        .AddScoped<IOrderModeCompanyMappingMigrationService, OrderModeCompanyMappingMigrationService>()
        .AddScoped<IDiscountMigrationService, DiscountMigrationService>()
        .AddScoped<IProductSizeMigrationService, ProductSizeMigrationService>()
        .AddScoped<IFlavourMigrationService, FlavourMigrationService>()
        .AddScoped<ICityMigrationService, CityMigrationService>()
        .AddScoped<IAreaMigrationService, AreaMigrationService>()
        .AddScoped<ISetupCompanySettingsMigrationService, SetupCompanySettingsMigrationService>();
    })
    .Build();

await host.RunAsync();