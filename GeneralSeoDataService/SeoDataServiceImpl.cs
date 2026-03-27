using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using PointofSaleModels.PGDatabaseModels;
using PointofSaleModels.Protos;
using static PointofSaleModels.Protos.GeneralSeoDataService;

namespace GeneralSeoDataService
{
    public class SeoDataServiceImpl(IDbContextFactory<RestaurantsContext> dbContextFactory) : GeneralSeoDataServiceBase
    {
        public override async Task<SeoDataList> GetSeoData(Domain request, ServerCallContext context)
        {
            var connectionString = await GetConnectionString(request.DomainName);
            var list = new SeoDataList();

            await foreach (var seoData in FetchSeoData(connectionString))
            {
                list.GeneralSeo.Add(seoData);
            }
            return list;
        }

        private static async IAsyncEnumerable<SeoData> FetchSeoData(string connectionString)
        {
            await using var dbContext = GetDbContext(connectionString);
            var generalSeoKeys = new List<string>()
        {
            "WEBSITE_META_TITLE",
            "HOMEPAGE_META_TITLE",
            "HOMEPAGE_META_DESCRIPTION",
            "H1_META",
            "BODY_CONTENT"
        };
            var settings = await dbContext.SetupMasterDetails
            .Join(dbContext.SetupCompanySettings, a => a.SetupDetailId, b => b.SetupDetailId, (a, b) => new { Id = a.SetupDetailId, Key = a.Flex1 ?? "", Value = b.SettingValue ?? "" })
            .Where(x => generalSeoKeys.Contains(x.Key))
            .ToListAsync();
            foreach (var entry in settings)
            {
                yield return new SeoData
                {
                    Id = entry.Id,
                    Name = entry.Key,
                    Value = entry.Value
                };
            }
        }

        private async Task<string> GetConnectionString(string domainName)
        {
            await using var context = await dbContextFactory.CreateDbContextAsync();
            var restaurant = await context.Restaurants.FirstOrDefaultAsync(r => r.DomainName == domainName);
            return restaurant?.ConnectionString ?? throw new Exception("Restaurant not found");
        }

        private static PgDbContext GetDbContext(string connectionString)
        {
            var options = new DbContextOptionsBuilder<PgDbContext>()
                .UseNpgsql(connectionString, options =>
                {
                    options.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorCodesToAdd: null);
                })
                .Options;
            return new PgDbContext(options);
        }
    }
}
