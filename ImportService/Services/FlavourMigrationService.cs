using ImportService.DatabaseContexts;
using ImportService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImportService.Services
{
    public class FlavourMigrationService(
        IDbContextFactory<SqlServerDbContext> sqlDbFactory) : IFlavourMigrationService
    {
        public async Task MigrateAsync(PostgresDbContext PgDb, int companyId = 0, CancellationToken ct = default)
        {
            await using var SqlDb = await sqlDbFactory.CreateDbContextAsync(ct);
            var flavours = await SqlDb.Flavours
                .Where(x => x.IsActive == true && x.CompanyId == companyId)
                .AsNoTracking()
                .ToListAsync(ct);
            await PgDb.Flavours.ExecuteDeleteAsync(ct);
            if (flavours.Count >= 1)
            {
                await PgDb.Flavours.AddRangeAsync(flavours, ct);
            }
        }
    }
}

