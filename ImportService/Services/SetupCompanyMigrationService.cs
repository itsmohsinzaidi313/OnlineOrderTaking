using ImportService.Data;
using ImportService.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ImportService.Services
{
    public class SetupCompanyMigrationService(SqlServerDbContext SqlDb) : ISetupCompanyMigrationService
    {
        public async Task MigrateSetupCompanyAsync(int companyId, PostgresDbContext PgDb, CancellationToken ct = default)
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
