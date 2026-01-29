using ImportService.Data;

namespace ImportService.Interfaces
{
    public interface IPaymentModeMigrationService
    {
        Task MigratePaymentModesAsync(int companyId, PostgresDbContext pgDb, CancellationToken ct = default);
    }
}


