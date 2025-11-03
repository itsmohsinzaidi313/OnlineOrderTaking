using Microsoft.EntityFrameworkCore;
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
    internal class RequestQueueAction(RestaurantErpWebContext dbContext, RabbitMqConnection connection) : IQueueAction
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
            Console.WriteLine("📂 Fetching menu items from database...");
            var connection = dbContext.Database.GetDbConnection();
            var command = connection.CreateCommand();

            command.CommandText = @"SELECT ""Id"", ""Name"", ""Price"" FROM ""Products""";
            if (connection.State == System.Data.ConnectionState.Open)
            {
                connection.Close();
            }
            connection.Open();
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                yield return new
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Price = reader.GetDecimal(reader.GetOrdinal("Price"))
                };
            }
            Console.WriteLine("✅ Menu items fetched successfully.");
        }
    }
}
