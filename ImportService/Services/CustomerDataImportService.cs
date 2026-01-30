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
            var customerPhones = await SqlDb.CustomerPhones
                .Where(x => x.IsActive == true && x.CompanyId == companyId)
                .AsNoTracking()
                .ToListAsync(ct);
            await PgDb.CustomerPhones.ExecuteDeleteAsync(ct);
            if (customerPhones.Count == 0)
            {
                return;
            }
            await PgDb.CustomerPhones.AddRangeAsync(customerPhones, ct);

            var customers = await SqlDb.Customers
                .Join(SqlDb.CustomerPhones
                      .Where(x => x.IsActive == true && x.CompanyId == companyId),
                        customer => customer.PhoneId,
                        phone => phone.PhoneId,
                        (customer, phone) => customer)
                .AsNoTracking()
                .GroupBy(x => x.CustomerId)
                .Select(x => x.First())
                .ToListAsync(ct);

            await PgDb.Customers.ExecuteDeleteAsync(ct);
            if (customers.Count == 0)
            {
                return;
            }
            await PgDb.Customers.AddRangeAsync(customers, ct);

            var customerAddressDetails = await SqlDb.CustomerAddressDetails
                .Join(
                    SqlDb.CustomerPhones
                    .Where(ph => ph.IsActive == true && ph.CompanyId == companyId),
                    address => address.PhoneId,
                    phone => phone.PhoneId,
                    (address, phone) => address
                )
                .AsNoTracking()
                .GroupBy(ad => ad.CustomerAddressId)
                .Select(g => g.First())
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
