using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using PointofSaleModels.PGDatabaseModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace PointofSaleModels.HealthChecks
{
    public class PostgresHealth(IDbContextFactory<RestaurantsContext> contextFactory) : IHealthCheck
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
