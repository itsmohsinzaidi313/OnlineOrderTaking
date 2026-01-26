using ImportService.Data;
using ImportService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImportService.Services
{
    public class SetupMasterDetailMigrationService(
        SqlServerDbContext sqlDb) : ISetupMasterDetailMigrationService
    {
        public async Task MigrateSetupMasterDetailAsync(int companyId, PostgresDbContext pgDb, CancellationToken ct = default)
        {
            var details = await sqlDb.SetupMasterDetails
                .Where(d => d.CompanyId == companyId && d.IsActive == true)
                .AsNoTracking()
                .ToListAsync(ct);
            await pgDb.SetupMasterDetails.ExecuteDeleteAsync(ct);
            if(details.Count == 0)
            {
                return;
            }
            await pgDb.SetupMasterDetails.AddRangeAsync(details, ct);
        }
    }
}


