namespace DataMigration.Application.Interfaces
{
    public interface ICityMigrationService
    {
        Task<int> MigrateCitiesAsync(CancellationToken ct = default);
    }
}

