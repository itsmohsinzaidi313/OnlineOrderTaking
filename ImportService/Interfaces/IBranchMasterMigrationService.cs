using ImportService.Data;

namespace ImportService.Interfaces
{
    public interface IBranchMasterMigrationService
    {
        Task MigrateBranchMasterAsync(int companyId, PostgresDbContext PgDb, CancellationToken ct = default);
    }
}
