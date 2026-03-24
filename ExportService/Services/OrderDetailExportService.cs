using ExportService.Interfaces;
using Microsoft.EntityFrameworkCore;
using PointofSaleModels.DatabaseContexts;

namespace ExportService.Services
{
    public class OrderDetailExportService(SqlServerDbContext sqlServerDb) : IOrderDetailExportService
    {
        public async Task ExportAsync(PostgresDbContext PgDb, CancellationToken ct = default)
        {
            var orderDetails = await PgDb.OrderDetails.ToListAsync(ct);
            await sqlServerDb.OrderDetails.AddRangeAsync(orderDetails, ct);
        }
    }
}
