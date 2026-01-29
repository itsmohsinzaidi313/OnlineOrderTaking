using ImportService.Data;
using ImportService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImportService.Services
{
    public class PaymentModeMigrationService(SqlServerDbContext sqlDb) : IPaymentModeMigrationService
    {
        public async Task MigratePaymentModesAsync(int companyId, PostgresDbContext pgDb, CancellationToken ct = default)
        {
            var source = await sqlDb.PaymentModes
                .Where(x => x.IsActive == true && x.CompanyId == companyId)
                .AsNoTracking()
                .ToListAsync(ct);

            await pgDb.PaymentModes.ExecuteDeleteAsync(ct);
            await pgDb.PaymentModes.AddRangeAsync(source, ct);
        }
    }
}


