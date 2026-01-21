namespace DataMigration.Application.Interfaces
{
    public interface IDataMigrationService
    {
        Task<int> MigrateTableAsync(string tableName, int batchSize = 5000, CancellationToken ct = default);
        Task<int> MigrateAllAsync(CancellationToken ct = default);
    }
}
