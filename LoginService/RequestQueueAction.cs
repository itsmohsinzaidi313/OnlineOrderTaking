using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Db = PointofSaleModels.PGDatabaseModels;
using PointofSaleModels.ServicePayloads;
using PointofSaleModels.Services;
using PointofSaleModels.Settings;
using App = PointofSaleModels.Application;

namespace LoginService
{
    internal class RequestQueueAction(ILogger<RequestQueueAction> logger, Implementation impl, IRabbitMqPublisher publisher, Db.PgDbContext context) : IQueueAction
    {
        public string QueueName() => RabbitMqQueues.LoginRequestQueue;

        public async Task OnMessage(ServicePayload svcpayload)
        {
            var payload = svcpayload.GetPayload<LoginServicePayload>();
            var phoneNumber = payload.Customer.Contact;
            var customerPhone = await context.CustomerPhones
                .FirstOrDefaultAsync(c => c.PhoneNumber == phoneNumber);
            var customer = await context.Customers.FirstOrDefaultAsync(c => c.PhoneId == customerPhone.PhoneId);
            var customerAddresses = await context.CustomerAddressDetails
                .Where(ca => ca.PhoneId == customerPhone.PhoneId)
                .ToListAsync();
            if (customerPhone is not null)
            {
                var responsePayload = new LoginServicePayload(payload)
                {
                    Customer = new App.Customer
                    {
                        Contact = customerPhone.PhoneNumber,
                        Name = customer.CustomerName,
                        PhoneId = customerPhone.PhoneId,
                        SelectedAddress = customerAddresses.FirstOrDefault(x => x.IsPrimary).CompleteAddress ,
                        Addresses = customerAddresses.Select(ca => ca.CompleteAddress).ToList(),
                    }
                };
                await publisher.PublishToQueueAsync(RabbitMqQueues.LoginResponseQueue, responsePayload);
            }
        }
    }
}
