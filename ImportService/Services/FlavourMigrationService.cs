using DataMigration.Application.Interfaces;
using DataMigration.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DataMigration.Application.Services
{
    public class FlavourMigrationService : IFlavourMigrationService
    {
        private readonly SqlServerDbContext _sqlDb;
        private readonly PostgresDbContext _pgDb;
        private readonly ILogger<FlavourMigrationService> _logger;

        public FlavourMigrationService(
            SqlServerDbContext sqlDb,
            PostgresDbContext pgDb,
            ILogger<FlavourMigrationService> logger)
        {
            _sqlDb = sqlDb;
            _pgDb = pgDb;
            _logger = logger;
        }

        public async Task<int> MigrateFlavoursAsync(int companyId, CancellationToken ct = default)
        {
            int migrated = 0;

            try
            {
                var flavours = await _sqlDb.Flavours
                    .Where(x => x.IsActive == true && (x.CompanyId == null || x.CompanyId == companyId))
                    .AsNoTracking()
                    .ToListAsync(ct);

                if (flavours == null || flavours.Count == 0)
                {
                    _logger.LogInformation("No Flavour rows to migrate for CompanyId={CompanyId}", companyId);
                    return 0;
                }

                await _pgDb.Flavours.ExecuteDeleteAsync(ct);
                await _pgDb.Flavours.AddRangeAsync(flavours, ct);

                migrated = await _pgDb.SaveChangesAsync(ct);
                _logger.LogInformation("✅ Flavour migration completed for CompanyId={CompanyId}. Rows affected: {Count}",
                    companyId, migrated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error migrating Flavours for CompanyId={CompanyId}", companyId);
                throw;
            }

            return migrated;
        }
    }
}

