namespace ImportService.Interfaces
{
    public interface IAreaMigrationService
    {
        Task<int> MigrateAreasAsync(int companyId, CancellationToken ct = default);
    }
}

