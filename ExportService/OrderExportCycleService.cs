using ExportService.DatabaseContexts;
using Microsoft.EntityFrameworkCore;
using PointofSaleModels.ServicePayloads;

namespace ExportService
{
    public class OrderExportCycleService(ILogger<OrderExportCycleService> logger, IDbContextFactory<RestaurantsDbContext> restaurantsContextFactory, OrderExportService exportService) : BackgroundService
    {
        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var restaurantsContext = await restaurantsContextFactory.CreateDbContextAsync(stoppingToken);
                    var restaurants = await restaurantsContext.Restaurants.ToListAsync(stoppingToken);
                    foreach (var restaurant in restaurants)
                    {
                        var connectionString = restaurant.ConnectionString;
                        var pgContext = exportService.GetDbContext(connectionString);
                        var orderNumbers = await pgContext.OrderMasters
                                                .Where(x => x.Exported == false)
                                                .Select(o => o.OrderNumber)
                                                .ToListAsync(stoppingToken);
                        if (orderNumbers.Count >= 1)
                        {
                            logger.LogInformation("Found {Count} unexported orders for restaurant {DomainName}.", orderNumbers.Count, restaurant.DomainName);
                        }
                        foreach (var orderNumber in orderNumbers)
                        {
                            try
                            {
                                var payload = new ExportServicePayload
                                {
                                    DomainName = restaurant.DomainName,
                                    OrderNumber = orderNumber,
                                    ExportType = "NewOrder"
                                };
                                await exportService.OnMessageHandler(payload, connectionString);
                            }
                            catch (Exception ex) { logger.LogError(ex.InnerException?.Message ?? ex.Message); }
                            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken); // Delay for 1 second between processing each order
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while executing the order export cycle.");
                }
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}
