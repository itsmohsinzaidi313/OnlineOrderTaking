namespace DataMigration.Application.Interfaces
{
	public interface IMenuMigrationService
	{
		Task<int> MigrateMenuAsync(int companyId,  CancellationToken ct = default);
	}
}


