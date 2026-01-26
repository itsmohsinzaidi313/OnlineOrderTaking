using ImportService.Data;

namespace ImportService.Interfaces
{
	public interface IMenuMigrationService
	{
		Task MigrateMenuAsync(int companyId, PostgresDbContext pgDb, CancellationToken ct = default);
	}
}


