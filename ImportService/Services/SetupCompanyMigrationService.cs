using ImportService.DatabaseContexts;
using ImportService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImportService.Services
{
    public class SetupCompanyMigrationService(SqlServerDbContext SqlDb) : ISetupCompanyMigrationService
    {
        public async Task MigrateAsync(PostgresDbContext PgDb, int companyId = 0, CancellationToken ct = default)
        {
            var source = await SqlDb.SetupCompanies
                .Where(x => x.CompanyId == companyId)
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);
            await PgDb.SetupCompanies.ExecuteDeleteAsync(ct);
            if (source == null) return;
            await PgDb.SetupCompanies.AddAsync(source, ct);
        }

    }
}
