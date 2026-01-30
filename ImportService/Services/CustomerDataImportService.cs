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

            var phoneIds = customerPhones.Select(x => x.PhoneId).ToList();

            var customers = await SqlDb.Customers
                .Where(x => x.IsActive == true && phoneIds.Contains(x.PhoneId ?? 0) && x.CompanyId == companyId)
                .AsNoTracking()
                .ToListAsync(ct);

            await PgDb.Customers.ExecuteDeleteAsync(ct);
            if (customers.Count == 0)
            {
                return;
            }
            await PgDb.Customers.AddRangeAsync(customers, ct);

            var customerAddressDetails = await SqlDb.CustomerAddressDetails
                .Where(x => x.IsActive == true && phoneIds.Contains(x.PhoneId ?? 0) && x.CompanyId == companyId)
                .AsNoTracking()
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
