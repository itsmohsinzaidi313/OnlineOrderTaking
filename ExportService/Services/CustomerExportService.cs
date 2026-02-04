using ExportService.Data;
using ExportService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExportService.Services
{
    public class CustomerExportService(SqlServerDbContext sqlServerDb) : ICustomerExportService
    {
        public async Task ExportAsync(PostgresDbContext postgresServerDb, CancellationToken ct = default)
        {
            var customerPhones = await postgresServerDb.CustomerPhones.AsNoTracking().ToListAsync(ct);
            var phoneIds = customerPhones.Select(cp => cp.PhoneId).ToList();

            var newPhoneIds = await sqlServerDb.CustomerPhones
                .AsNoTracking()
                .Where(cp => !phoneIds.Contains(cp.PhoneId))
                .Select(cp => cp.PhoneId)
                .ToListAsync(ct);

            var newCustomerPhones = customerPhones
                .Where(cp => newPhoneIds.Contains(cp.PhoneId))
                .ToList();

            var customerAddresses = await postgresServerDb.CustomerAddressDetails
                .Where(x => newPhoneIds.Contains(x.PhoneId ?? 0))
                .AsNoTracking()
                .ToListAsync(ct);

            var customers = await postgresServerDb.Customers
                .Where(x => newPhoneIds.Contains(x.PhoneId ?? 0))
                .AsNoTracking()
                .ToListAsync(ct);

            await sqlServerDb.CustomerPhones.AddRangeAsync(newCustomerPhones, ct);
            await sqlServerDb.CustomerAddressDetails.AddRangeAsync(customerAddresses, ct);
            await sqlServerDb.Customers.AddRangeAsync(customers, ct);
        }
    }
}
