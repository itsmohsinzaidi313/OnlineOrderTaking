using ImportService.Interfaces;
using Microsoft.EntityFrameworkCore;
using PointofSaleModels.DatabaseContexts;

namespace ImportService.Services
{
    public class PaymentModeMigrationService(SqlServerDbContext sqlDb) : IPaymentModeMigrationService
    {
        public async Task MigrateAsync(PostgresDbContext pgDb, int companyId = 0, CancellationToken ct = default)
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


