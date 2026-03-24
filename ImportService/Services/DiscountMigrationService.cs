using ImportService.Interfaces;
using Microsoft.EntityFrameworkCore;
using PointofSaleModels.DatabaseContexts;

namespace ImportService.Services
{
    public class DiscountMigrationService(
        SqlServerDbContext sqlDb) : IDiscountMigrationService
    {
        public async Task MigrateAsync(PostgresDbContext pgDb, int companyId = 0, CancellationToken ct = default)
        {
            // 1) Migrate main Discount records for this company
            var discounts = await sqlDb.Discounts
                .Where(x => x.IsActive == true && x.CompanyId == companyId)
                .AsNoTracking()
                .ToListAsync(ct);

            await pgDb.Discounts.ExecuteDeleteAsync(ct);
            if (discounts.Count >= 1)
            {
                await pgDb.Discounts.AddRangeAsync(discounts, ct);
            }
            else
            {
                return;
            }

            //var discountIds = discounts.Select(x => x.DiscountId).ToHashSet();

            // 2) Migrate DiscountDayMappings for the migrated discounts
            var dayMappings = await sqlDb.DiscountDayMappings
                .Join(sqlDb.Discounts.Where(x => x.CompanyId == companyId && x.IsActive == true),
                      a => a.DiscountId,
                      b => b.DiscountId,
                      (ddm, d) => ddm)
                .AsNoTracking()
                .ToListAsync(ct);
            await pgDb.DiscountDayMappings.ExecuteDeleteAsync(ct);
            await pgDb.DiscountDayMappings.AddRangeAsync(dayMappings, ct);



            // 3) Migrate DiscountBranchMappings for the migrated discounts
            var branchMappings = await sqlDb.DiscountBranchMappings
                .Join(sqlDb.Discounts.Where(x => x.CompanyId == companyId && x.IsActive == true),
                      a => a.DiscountId,
                      b => b.DiscountId,
                      (dbm, d) => dbm)
                .AsNoTracking()
                .ToListAsync(ct);

            await pgDb.DiscountBranchMappings.ExecuteDeleteAsync(ct);
            await pgDb.DiscountBranchMappings.AddRangeAsync(branchMappings, ct);

            // 4) Migrate DiscountOrderTypeMappings for the migrated discounts
            var orderTypeMappings = await sqlDb.DiscountOrderTypeMappings
                .Join(sqlDb.Discounts.Where(x => x.CompanyId == companyId && x.IsActive == true),
                      a => a.DiscountId,
                      b => b.DiscountId,
                      (otm, d) => otm)
                .AsNoTracking()
                .ToListAsync(ct);

            await pgDb.DiscountOrderTypeMappings.ExecuteDeleteAsync(ct);
            await pgDb.DiscountOrderTypeMappings.AddRangeAsync(orderTypeMappings, ct);

            // 5) Migrate DiscountOrderModeMappings for the migrated discounts
            var orderModeMappings = await sqlDb.DiscountOrderModeMappings
                .Join(sqlDb.Discounts.Where(x => x.CompanyId == companyId && x.IsActive == true),
                      a => a.DiscountId,
                      b => b.DiscountId,
                      (omm, d) => omm)
                .AsNoTracking()
                .ToListAsync(ct);

            await pgDb.DiscountOrderModeMappings.ExecuteDeleteAsync(ct);
            await pgDb.DiscountOrderModeMappings.AddRangeAsync(orderModeMappings, ct);

            // 6) Migrate DiscountProductDetailMappings for the migrated discounts
            var productDetailMappings = await sqlDb.DiscountProductDetailMappings
                .Join(sqlDb.Discounts.Where(x => x.CompanyId == companyId && x.IsActive == true),
                      a => a.DiscountId,
                      b => b.DiscountId,
                      (pdm, d) => pdm)
                .AsNoTracking()
                .ToListAsync(ct);

            await pgDb.DiscountProductDetailMappings.ExecuteDeleteAsync(ct);
            await pgDb.DiscountProductDetailMappings.AddRangeAsync(productDetailMappings, ct);

        }
    }
}
