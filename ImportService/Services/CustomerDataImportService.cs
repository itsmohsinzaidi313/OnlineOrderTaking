using ImportService.Data;
using ImportService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImportService.Services
{
    public class CustomerDataImportService(
        SqlServerDbContext SqlDb) : ICustomerDataImportService
    {
        public async Task MigrateCustomerDataAsync(int companyId, PostgresDbContext PgDb, CancellationToken ct = default)
        {
            var customerPhonesQuery = SqlDb.CustomerPhones
            var customerPhones = await SqlDb.CustomerPhones
                .Where(x => x.IsActive == true && x.CompanyId == companyId)
                .AsNoTracking()
                .Where(x => x.IsActive == true && x.CompanyId == companyId);

            var customerPhones = await customerPhonesQuery.ToListAsync(ct);
                .ToListAsync(ct);
            await PgDb.CustomerPhones.ExecuteDeleteAsync(ct);
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

            await PgDb.Customers.ExecuteDeleteAsync(ct);
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
        }
    }
}
