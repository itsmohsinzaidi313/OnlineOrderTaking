using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PointofSaleModels.DatabaseModels;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;

namespace GetMenuService
{
    internal class RequestQueueAction(ILogger<RequestQueueAction> logger, RestaurantErpWebContext dbContext, IRabbitMqPublisher publisher) : IQueueAction
    {
        public string QueueName() => RabbitMqQueues.MenuRequestQueue;

        public async Task OnMessage(RabbitMqTransport transport)
        {
            object payload;
            try
            {
                var responsePayload = await GetMenuItemsAsync();
                payload = responsePayload;
            }
            catch (Exception ex)
            {
                // Log full exception (stack trace and inner exceptions) to help diagnose stream/connection issues
                logger.LogError(ex, "Failed to fetch menu items.");
                payload = new
                {
                    error = true,
                    message = "Failed to fetch menu items.",
                    details = ex.ToString()
                };
            }

            var response = new RabbitMqTransport
            {
                ConnectionId = transport.ConnectionId,
                // Preserve the UserId so the Gateway can route responses by user when present
                UserId = transport.UserId,
                Route = "menu.response",
                CompanyId = transport.CompanyId,
                BranchId = transport.BranchId,
                Payload = payload
            };
            await publisher.PublishToQueueAsync(RabbitMqQueues.MenuResponseQueue, response);
        }

        private async Task<List<object>> GetMenuItemsAsync()
        {
            logger.LogInformation("📂 Fetching menu items from database...");

            var results = new List<object>();

            var connection = dbContext.Database.GetDbConnection();

            var command = connection.CreateCommand();
            command.CommandText = @"SELECT ""Id"", ""Name"", ""Price"" FROM ""Products""";

            try
            {
                if (connection.State != System.Data.ConnectionState.Open)
                {
                    logger.LogInformation("Opening database connection...");
                    await connection.OpenAsync();
                }
                else
                {
                    logger.LogInformation("Database connection already open, reusing connection.");
                }

                logger.LogInformation("Executing SQL command...");
                await using var reader = await command.ExecuteReaderAsync();
                int rowCount = 0;
                while (await reader.ReadAsync())
                {
                    rowCount++;
                    var id = reader.GetInt32(reader.GetOrdinal("Id"));
                    var nameOrdinal = reader.GetOrdinal("Name");
                    string? name = reader.IsDBNull(nameOrdinal) ? null : reader.GetString(nameOrdinal);
                    var priceOrdinal = reader.GetOrdinal("Price");
                    decimal? price = reader.IsDBNull(priceOrdinal) ? null : reader.GetDecimal(priceOrdinal);

                    logger.LogTrace("Fetched row {Row}: Id={Id}, Name={Name}, Price={Price}", rowCount, id, name, price);

                    results.Add(new
                    {
                        Id = id,
                        Name = name,
                        Price = price
                    });
                }

                logger.LogInformation("✅ Menu items fetched successfully. Total items: {RowCount}", rowCount);
            }
            finally
            {
                try
                {
                    if (connection.State == System.Data.ConnectionState.Open)
                    {
                        await connection.CloseAsync();
                    }
                }
                catch (Exception closeEx)
                {
                    // Log but do not rethrow; closing failure shouldn't crash the enumerator caller
                    logger.LogWarning(closeEx, "Failed to close database connection after fetching menu items.");
                }
                try { command.Dispose(); } catch { }
            }

            return results;
        }
    }
}
