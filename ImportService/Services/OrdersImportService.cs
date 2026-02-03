using ImportService.Data;
using ImportService.Entities;
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
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            await postgresDbContext.OrderMasters.AddRangeAsync(orderMasterList, cancellationToken);
        }
    }
}
