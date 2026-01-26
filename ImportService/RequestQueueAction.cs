using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Db = PointofSaleModels.PGDatabaseModels;

namespace ImportService
{
    internal class RequestQueueAction(ILogger<RequestQueueAction> logger, IRabbitMqPublisher publisher) : IQueueAction
    {
        public string QueueName() => RabbitMqQueues.ImportRequestQueue;

        public async Task OnMessage(string transport)
        {
            ImportServicePayload response;
            try
            {
                var servicePayload = System.Text.Json.JsonSerializer.Deserialize<ImportServicePayload>(transport);
                var companyId = servicePayload!.RestaurantId;


                response = new ImportServicePayload
                {
                    Success = true,
                    Message = "Data imported successfully."
                };
            }
            catch (Exception ex)
            {
                response = new ImportServicePayload
                {
                    Success = false,
                    Message = $"Error occurred while processing the request.\n{ex.Message}"
                };
                logger.LogError(ex, "Error occurred while processing import request.");
            }
            await publisher.PublishToQueueAsync(RabbitMqQueues.ImportResponseQueue, response);
        }
    }
}
