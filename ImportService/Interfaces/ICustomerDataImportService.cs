using ImportService.Data;

namespace ImportService.Interfaces
{
    public interface ICustomerDataImportService
    {
        Task MigrateCustomerDataAsync(int companyId, PostgresDbContext PgDb, CancellationToken ct = default);
    }
}
