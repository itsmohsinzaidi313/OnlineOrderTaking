using ImportService.Data;
using ImportService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImportService.Services
{
    public class OrdersImportService(SqlServerDbContext sqlServerDb) : IOrdersImportService
    {
        public async Task MigrateOrdersAsync(int companyId, PostgresDbContext postgresDbContext, CancellationToken cancellationToken)
        {

            var orderMasterList = await sqlServerDb.OrderMasters
                .Include(x => x.OrderDetails)
                .Where(om => om.CompanyId == companyId)
                .OrderByDescending(om => om.OrderMasterId)
                .Take(10)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
            await postgresDbContext.OrderMasters
                .Include(x => x.OrderDetails)
                .ExecuteDeleteAsync(cancellationToken);
            await postgresDbContext.OrderMasters.AddRangeAsync(orderMasterList, cancellationToken);
        }
    }
}
