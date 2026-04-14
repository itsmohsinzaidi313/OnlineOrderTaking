using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using PointofSaleModels.PGDatabaseModels;
using PointofSaleModels.Protos;
using PointofSaleModels.Services;
using static PointofSaleModels.Protos.GeneralSeoDataService;

namespace GeneralSeoDataService
{
    public class SeoDataServiceImpl(IConnectionStringResolver resolver) : GeneralSeoDataServiceBase
    {
        public override async Task<SeoDataList> GetSeoData(Domain request, ServerCallContext context)
        {
            var list = new SeoDataList();

            await foreach (var seoData in FetchSeoData(request.DomainName))
            {
                list.GeneralSeo.Add(seoData);
            }
            return list;
        }

        private async IAsyncEnumerable<SeoData> FetchSeoData(string domainName)
        {
            await using var dbContext = await resolver.ResolveAndGetReadOnlyDbContextAsync(domainName);
            var generalSeoKeys = new List<string>()
        {
            "WEBSITE_META_TITLE",
            "HOMEPAGE_META_TITLE",
            "HOMEPAGE_META_DESCRIPTION",
            "H1_META",
            "BODY_CONTENT",
            "UPLOAD_LOGO",
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
                    Value = entry.Value == "UPLOAD_LOGO" ? "RESTAURANT_LOGO" : entry.Value
                };
            }
        }
    }
}
