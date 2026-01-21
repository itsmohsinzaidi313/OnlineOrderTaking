namespace ImportService.Interfaces
{
    public interface IGSTMigrationService
    {
        Task<int> MigrateGSTsAsync(int companyId, CancellationToken ct = default);
    }
}

