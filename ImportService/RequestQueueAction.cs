using Azure;
using ImportService.Data;
using ImportService.Interfaces;
using Microsoft.Extensions.Logging;
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
    internal class RequestQueueAction(ILogger<RequestQueueAction> logger, IRabbitMqPublisher publisher, PostgresDbContext postgresDbContext,
        ISetupCompanyMigrationService service_setupCompany,
        IBranchMasterMigrationService service_branchMaster,
        IMenuMigrationService service_menu,
        ISetupMasterMigrationService service_setupMaster,
        ISetupMasterDetailMigrationService service_setupMasterDetail,
        IPaymentModeMigrationService service_paymentMode,
        IDiscountMigrationService service_discount,
        IProductSizeMigrationService service_productSize,
        IFlavourMigrationService service_flavour,
        ICityMigrationService service_city,
        IAreaMigrationService service_area,
        ISetupCompanySettingsMigrationService service_setupCompanySettings) : IQueueAction
    {
        public string QueueName() => RabbitMqQueues.ImportRequestQueue;

        public async Task OnMessage(string transport)
        {
            ImportServicePayload response;
            try
            {
                var servicePayload = System.Text.Json.JsonSerializer.Deserialize<ImportServicePayload>(transport);
                var companyId = servicePayload!.RestaurantId;
                var dbCreated = await postgresDbContext.Database.EnsureCreatedAsync();
                if (!dbCreated)
                {
                }

                await service_setupCompany.MigrateSetupCompanyAsync(companyId);

                await service_setupMaster.MigrateSetupMasterAsync();

                await service_setupMasterDetail.MigrateSetupMasterDetailAsync(companyId);

                await service_city.MigrateCitiesAsync();

                await service_area.MigrateAreasAsync(companyId);

                await service_branchMaster.MigrateBranchMasterAsync(companyId);

                await service_productSize.MigrateProductSizesAsync(companyId);

                await service_flavour.MigrateFlavoursAsync(companyId);

                await service_menu.MigrateMenuAsync(companyId);

                await service_paymentMode.MigratePaymentModesAsync(companyId);

                await service_setupCompanySettings.MigrateSetupCompanySettingsAsync(companyId);

                await service_discount.MigrateDiscountsAsync(companyId);

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
