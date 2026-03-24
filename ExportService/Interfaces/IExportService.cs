using PointofSaleModels.DatabaseContexts;

namespace ExportService.Interfaces
{
    public interface IExportService
    {
        Task ExportAsync(PostgresDbContext postgresServerDb, CancellationToken ct = default);
    }
}
