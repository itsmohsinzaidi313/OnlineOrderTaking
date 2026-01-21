using DataMigration.Application.Interfaces;
using DataMigration.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DataMigration.Application.Services
{
    public class SetupCompanyMigrationService : ISetupCompanyMigrationService
    {
        private readonly SqlServerDbContext _sqlDb;
        private readonly PostgresDbContext _pgDb;
        private readonly ILogger<SetupCompanyMigrationService> _logger;

        public SetupCompanyMigrationService(SqlServerDbContext sqlDb, PostgresDbContext pgDb, ILogger<SetupCompanyMigrationService> logger)
        {
            _sqlDb = sqlDb;
            _pgDb = pgDb;
            _logger = logger;
        }

        public async Task<int> MigrateSetupCompanyAsync(int companyId, CancellationToken ct = default)
        {
            var source = await _sqlDb.SetupCompanies
                .Where(x => x.CompanyId == companyId)
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (source == null)
            {
                _logger.LogWarning("No company found with ID {CompanyId}", companyId);
                return 0;
            }

            await _pgDb.SetupCompanies.ExecuteDeleteAsync(ct);
            await _pgDb.SetupCompanies.AddAsync(source, ct);

            return await _pgDb.SaveChangesAsync(ct);
        }

    }
}
