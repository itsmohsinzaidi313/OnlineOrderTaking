using ImportService.Interfaces;
using Microsoft.EntityFrameworkCore;
using PointofSaleModels.DatabaseContexts;

namespace ImportService.Services
{
    public class CustomerDataImportService(
        SqlServerDbContext SqlDb) : ICustomerDataImportService
    {
        public async Task MigrateAsync(PostgresDbContext PgDb, int companyId = 0, CancellationToken ct = default)
        {
            var customerPhonesQuery = SqlDb.CustomerPhones
                .Where(x => x.IsActive == true && x.CompanyId == companyId)
                .AsNoTracking();

            var customerPhones = await customerPhonesQuery.ToListAsync(ct);
            
            if (customerPhones.Count == 0)
            {
                return;
            }
            await PgDb.CustomerPhones.AddRangeAsync(customerPhones, ct);

            var customers = await SqlDb.Customers
                .AsNoTracking()
                .Join(customerPhonesQuery,
                    customer => customer.PhoneId,
                    phone => phone.PhoneId,
                    (customer, _) => customer)
                .Distinct()
                .ToListAsync(ct);

            if (customers.Count == 0)
            {
                return;
            }
            await PgDb.Customers.AddRangeAsync(customers, ct);

            var customerAddressDetails = await SqlDb.CustomerAddressDetails
                .AsNoTracking()
                .Join(
                    customerPhonesQuery,
                    address => address.PhoneId,
                    phone => phone.PhoneId,
                    (address, _) => address)
                .Distinct()
                .ToListAsync(ct);

            await PgDb.CustomerAddressDetails.ExecuteDeleteAsync(ct);
            if (customerAddressDetails.Count == 0)
            {
                return;
            }
            await PgDb.CustomerAddressDetails.AddRangeAsync(customerAddressDetails, ct);
            await PgDb.Customers.ExecuteDeleteAsync(ct);
            await PgDb.CustomerPhones.ExecuteDeleteAsync(ct);
        }
    }
}
