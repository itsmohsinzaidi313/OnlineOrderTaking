using ImportService.Data;

namespace ImportService.Interfaces
{
    public interface IUserLoginMigrationService
    {
        Task MigrateUserLoginAsync(int companyId, PostgresDbContext pgDb, CancellationToken ct = default);
    }
}
