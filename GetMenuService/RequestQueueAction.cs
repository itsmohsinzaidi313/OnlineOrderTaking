using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PointofSaleModels.DatabaseModels;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetMenuService
{
    internal class RequestQueueAction(ILogger<RequestQueueAction> logger, RestaurantErpWebContext dbContext, RabbitMqConnection connection) : IQueueAction
    {
        public string QueueName() => RabbitMqQueues.MenuRequestQueue;
        public async Task OnMessage(RabbitMqTransport transport)
        {
            List<object> responsePayload = [];
            foreach (var item in GetMenuItems())
            {
                responsePayload.Add(item);
            }
            var response = new RabbitMqTransport
            {
                ConnectionId = transport.ConnectionId,
                // Preserve the UserId so the Gateway can route responses by user when present
                UserId = transport.UserId,
                Route = "menu.response",
                CompanyId = transport.CompanyId,
                BranchId = transport.BranchId,
                Payload = responsePayload
            };
            await connection.PublishResponseAsync(response, RabbitMqQueues.MenuResponseQueue);
        }
        private IEnumerable<object> GetMenuItems()
        {
            logger.LogInformation("📂 Fetching menu items from database...");
            var connection = dbContext.Database.GetDbConnection();
            var command = connection.CreateCommand();

            logger.LogInformation("Preparing SQL command: {CommandText}", command.CommandText);
            command.CommandText = @"SELECT ""Id"", ""Name"", ""Price"" FROM ""Products""";
            if (connection.State == System.Data.ConnectionState.Open)
            {
            logger.LogInformation("Database connection is open, closing before proceeding.");
            connection.Close();
            }
            logger.LogInformation("Opening database connection...");
            connection.Open();
            logger.LogInformation("Executing SQL command...");
            var reader = command.ExecuteReader();
            int rowCount = 0;
            while (reader.Read())
            {
            rowCount++;
            logger.LogTrace("Fetched row {Row}: Id={Id}, Name={Name}, Price={Price}",
                rowCount,
                reader.GetInt32(reader.GetOrdinal("Id")),
                reader.GetString(reader.GetOrdinal("Name")),
                reader.GetDecimal(reader.GetOrdinal("Price"))
            );
            yield return new
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                Price = reader.GetDecimal(reader.GetOrdinal("Price"))
            };
            }
            logger.LogInformation("✅ Menu items fetched successfully. Total items: {RowCount}", rowCount);
        }
    }
}
