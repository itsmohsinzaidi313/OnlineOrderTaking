using ImportService.DatabaseContexts;
using ImportService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImportService.Services
{
    public class GSTMigrationService(IDbContextFactory<SqlServerDbContext> sqlDbFactory) : IGSTMigrationService
    {
        public async Task MigrateAsync(PostgresDbContext pgDb, int companyId = 0, CancellationToken ct = default)
        {
            await using var sqlDb = await sqlDbFactory.CreateDbContextAsync(ct);
            var source = await sqlDb.GSTs
                .Where(x => x.IsActive == true && x.CompanyId == companyId)
                .AsNoTracking()
                .ToListAsync(ct);

            await pgDb.GSTs.ExecuteDeleteAsync(ct);
            await pgDb.GSTs.AddRangeAsync(source, ct);
        }
    }
}

