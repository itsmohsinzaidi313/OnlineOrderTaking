using ImportService.Data;
using ImportService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImportService.Services
{
    public class PaymentModeMigrationService(SqlServerDbContext sqlDb, PostgresDbContext pgDb, ILogger<PaymentModeMigrationService> logger) : IPaymentModeMigrationService
    {
        public async Task<int> MigratePaymentModesAsync(int companyId, CancellationToken ct = default)
        {
            var source = await sqlDb.PaymentModes
                .Where(x => x.IsActive == true && x.CompanyId == companyId)
                .AsNoTracking()
                .ToListAsync(ct);

            if (source == null || source.Count == 0)
            {
                logger.LogInformation("No PaymentMode rows to migrate for CompanyId={CompanyId}", companyId);
                return 0;
            }

            int migrated = 0;

            await pgDb.PaymentModes.ExecuteDeleteAsync(ct);
            await pgDb.PaymentModes.AddRangeAsync(source, ct);

            await pgDb.SaveChangesAsync(ct);
            logger.LogInformation("Migrated {Count} PaymentMode rows for CompanyId={CompanyId}", migrated, companyId);
            return migrated;
        }
    }
}


