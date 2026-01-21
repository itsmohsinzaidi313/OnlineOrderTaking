namespace DataMigration.Application.Interfaces
{
    public interface IFlavourMigrationService
    {
        Task<int> MigrateFlavoursAsync(int companyId, CancellationToken ct = default);
    }
}

