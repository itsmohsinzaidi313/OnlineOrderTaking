using ExportService.Data;
using ExportService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExportService.Services
{
    public class OrderMasterExportService(SqlServerDbContext sqlServerDb) : IOrderMasterExportService
    {
        public async Task ExportAsync(PostgresDbContext postgresServerDb, CancellationToken ct = default)
        {
            var orders = await postgresServerDb.OrderMasters.AsNoTracking().ToListAsync(ct);
            await sqlServerDb.OrderMasters.AddRangeAsync(orders, ct);
        }
    }
}
