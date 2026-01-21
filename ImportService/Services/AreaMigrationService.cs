using DataMigration.Application.Interfaces;
using DataMigration.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DataMigration.Application.Services
{
    public class AreaMigrationService : IAreaMigrationService
    {
        private readonly SqlServerDbContext _sqlDb;
        private readonly PostgresDbContext _pgDb;
        private readonly ILogger<AreaMigrationService> _logger;

        public AreaMigrationService(
            SqlServerDbContext sqlDb,
            PostgresDbContext pgDb,
            ILogger<AreaMigrationService> logger)
        {
            _sqlDb = sqlDb;
            _pgDb = pgDb;
            _logger = logger;
        }

        public async Task<int> MigrateAreasAsync(int companyId, CancellationToken ct = default)
        {
            int migrated = 0;

            try
            {
                var areas = await _sqlDb.Areas
                    .Where(x => x.IsActive == true && x.CompanyId == companyId)
                    .AsNoTracking()
                    .ToListAsync(ct);

                if (areas == null || areas.Count == 0)
                {
                    _logger.LogInformation("No Area rows to migrate for CompanyId={CompanyId}", companyId);
                    return 0;
                }
                await _pgDb.Areas.ExecuteDeleteAsync(ct);

                await _pgDb.AddRangeAsync(areas, ct);

                migrated = await _pgDb.SaveChangesAsync(ct);
                _logger.LogInformation("✅ Area migration completed for CompanyId={CompanyId}. Rows affected: {Count}",
                    companyId, migrated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error migrating Areas for CompanyId={CompanyId}", companyId);
                throw;
            }

            return migrated;
        }
    }
}

