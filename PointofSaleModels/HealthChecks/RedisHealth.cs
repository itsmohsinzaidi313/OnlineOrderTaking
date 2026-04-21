using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace PointofSaleModels.HealthChecks
{
    public class RedisHealth(IConnectionMultiplexer multiplexer) : IHealthCheck
    {
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var ping = await multiplexer.GetDatabase().PingAsync();

                if (ping < TimeSpan.FromSeconds(1))
                    return HealthCheckResult.Healthy("Redis is responsive.");
                else
                    return HealthCheckResult.Degraded($"Redis ping is slow: {ping.TotalMilliseconds} ms.");

            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(exception: ex);
            }
        }
    }
}