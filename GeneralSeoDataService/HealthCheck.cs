using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using PointofSaleModels.PGDatabaseModels;

namespace GeneralSeoDataService
{
    public class HealthCheck(IDbContextFactory<RestaurantsContext> contextFactory) : IHealthCheck
    {
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                using var dbContext = contextFactory.CreateDbContext();
                if (await dbContext.Database.CanConnectAsync(cancellationToken))
                {
                    return HealthCheckResult.Healthy();
                }
                else
                {
                    return HealthCheckResult.Unhealthy();
                }
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(exception: ex);
            }
        }
    }
}
