using ImportService.Data;

namespace ImportService.Interfaces
{
    public interface IOrdersImportService
    {
        Task MigrateOrdersAsync(int companyId, PostgresDbContext postgresDbContext, CancellationToken cancellationToken);
    }
}
