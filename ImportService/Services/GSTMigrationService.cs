using ImportService.Data;
using ImportService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImportService.Services
{
    public class GSTMigrationService : IGSTMigrationService
    {
        private readonly SqlServerDbContext _sqlDb;
        private readonly PostgresDbContext _pgDb;
        private readonly ILogger<GSTMigrationService> _logger;

        public GSTMigrationService(SqlServerDbContext sqlDb, PostgresDbContext pgDb, ILogger<GSTMigrationService> logger)
        {
            _sqlDb = sqlDb;
            _pgDb = pgDb;
            _logger = logger;
        }

        public async Task<int> MigrateGSTsAsync(int companyId, CancellationToken ct = default)
        {
            var source = await _sqlDb.GSTs
                .Where(x => x.IsActive == true && x.CompanyId == companyId)
                .AsNoTracking()
                .ToListAsync(ct);

            if (source == null || source.Count == 0)
            {
                _logger.LogInformation("No GST rows to migrate for CompanyId={CompanyId}", companyId);
                return 0;
            }

            int migrated = 0;
            await _pgDb.GSTs.ExecuteDeleteAsync(ct);
            await _pgDb.GSTs.AddRangeAsync(source);

            migrated = await _pgDb.SaveChangesAsync(ct);
            _logger.LogInformation("Migrated {Count} GST rows for CompanyId={CompanyId}", migrated, companyId);
            return migrated;
        }
    }
}

