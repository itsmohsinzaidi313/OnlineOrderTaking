using ImportService.DatabaseContexts;
using ImportService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImportService.Services
{
    public class PaymentModeMigrationService(IDbContextFactory<SqlServerDbContext> sqlDbFactory) : IPaymentModeMigrationService
    {
        public async Task MigrateAsync(PostgresDbContext pgDb, int companyId = 0, CancellationToken ct = default)
        {
            await using var sqlDb = await sqlDbFactory.CreateDbContextAsync(ct);
            var source = await sqlDb.PaymentModes
                .Where(x => x.IsActive == true && x.CompanyId == companyId)
                .AsNoTracking()
                .ToListAsync(ct);

            await pgDb.PaymentModes.ExecuteDeleteAsync(ct);
            await pgDb.PaymentModes.AddRangeAsync(source, ct);
        }
    }
}


