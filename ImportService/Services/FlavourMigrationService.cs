using ImportService.Data;
using ImportService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImportService.Services
{
    public class FlavourMigrationService(
        SqlServerDbContext SqlDb) : IFlavourMigrationService
    {
        public async Task MigrateFlavoursAsync(int companyId, PostgresDbContext PgDb, CancellationToken ct = default)
        {
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

