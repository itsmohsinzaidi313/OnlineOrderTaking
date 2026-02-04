using ExportService.Data;

namespace ExportService.Interfaces
{
    public interface IExportService
    {
        Task ExportAsync(PostgresDbContext postgresServerDb, CancellationToken ct = default);
    }
}
