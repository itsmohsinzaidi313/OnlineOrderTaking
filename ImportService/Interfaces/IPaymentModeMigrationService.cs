namespace DataMigration.Application.Interfaces
{
    public interface IPaymentModeMigrationService
    {
        Task<int> MigratePaymentModesAsync(int companyId, CancellationToken ct = default);
    }
}


