using ExportService.DatabaseContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using PointofSaleModels.PGDatabaseModels;

namespace ExportService
{
    public class HealthCheck(IDbContextFactory<SqlServerDbContext> sqlContextFactory, IDbContextFactory<RestaurantsContext> pgContextFactory) : IHealthCheck
    {
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                using var sqlContext = sqlContextFactory.CreateDbContext();
                using var pgContext = pgContextFactory.CreateDbContext();
                if (await pgContext.Database.CanConnectAsync(cancellationToken) && await sqlContext.Database.CanConnectAsync(cancellationToken))
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
            //bool pgHealthy = false;
            //bool sqlHealthy = false;
            //string pgExceptionMessage = string.Empty;
            //string sqlExceptionMessage = string.Empty;
            //try
            //{
            //    using var pgContext = pgContextFactory.CreateDbContext();

            //    if (await pgContext.Database.CanConnectAsync(cancellationToken))
            //    {
            //        pgHealthy = true;
            //    }

            //}
            //catch (Exception ex)
            //{
            //    pgExceptionMessage = $"PostgreSQL connection failed: {ex.Message}";
            //}

            //try
            //{
            //    using var sqlContext = sqlContextFactory.CreateDbContext();

            //    if (await sqlContext.Database.CanConnectAsync(cancellationToken))
            //    {
            //        sqlHealthy = true;
            //    }

            //}
            //catch (Exception ex)
            //{
            //    sqlExceptionMessage = $"SQL Server connection failed: {ex.Message}";
            //}

            //if (pgHealthy && sqlHealthy)
            //{
            //    return HealthCheckResult.Healthy();
            //}

            //var errorMessage = string.Join(" | ", new[] { pgExceptionMessage, sqlExceptionMessage }.Where(msg => !string.IsNullOrEmpty(msg)));
            //return HealthCheckResult.Unhealthy(errorMessage);
        }
    }
}
