using ImportService.Data;
using ImportService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImportService.Services
{
    public class GSTMigrationService(SqlServerDbContext sqlDb) : IGSTMigrationService
    {
        public async Task MigrateGSTsAsync(int companyId, PostgresDbContext pgDb, CancellationToken ct = default)
        {
            var source = await sqlDb.GSTs
                .Where(x => x.IsActive == true && x.CompanyId == companyId)
                .AsNoTracking()
                .ToListAsync(ct);

            await pgDb.GSTs.ExecuteDeleteAsync(ct);
            await pgDb.GSTs.AddRangeAsync(source, ct);
        }
    }
}

