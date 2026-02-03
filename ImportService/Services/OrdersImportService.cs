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
                .Where(om => om.CompanyId == companyId)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
            var orderDetailList = await sqlServerDb.OrderDetails
                .AsNoTracking()
                .Join(sqlServerDb.OrderMasters, od => od.OrderMasterId, om => om.OrderMasterId, (od, om) => new { od, om })
                .Where(joined => joined.om.CompanyId == companyId)
                .Select(joined => joined.od)
                .ToListAsync(cancellationToken);

            // Map and insert into PostgreSQL

            await postgresDbContext.OrderMasters.AddRangeAsync(orderMasterList, cancellationToken);
            await postgresDbContext.OrderDetails.AddRangeAsync(orderDetailList, cancellationToken);
        }
    }
}
