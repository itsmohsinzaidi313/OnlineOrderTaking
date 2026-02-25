using ImportService.Data;
using ImportService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImportService.Services
{
    public class OrdersImportService(SqlServerDbContext sqlServerDb) : IOrdersImportService
    {
        public async Task MigrateAsync(PostgresDbContext postgresDbContext, int companyId = 0, CancellationToken ct = default)
        {

            var orderMasterList = await sqlServerDb.OrderMasters
                .Include(x => x.OrderDetails)
                .Where(om => om.CompanyId == companyId)
                .OrderByDescending(om => om.OrderMasterId)
                .Take(10)
                .AsNoTracking()
                .ToListAsync(ct);
            await postgresDbContext.OrderMasters
                .Include(x => x.OrderDetails)
                .ExecuteDeleteAsync(ct);
            await postgresDbContext.OrderMasters.AddRangeAsync(orderMasterList, ct);
        }
    }
}
