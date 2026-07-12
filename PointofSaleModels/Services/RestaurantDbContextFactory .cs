using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using PointofSaleModels.PGDatabaseModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace PointofSaleModels.Services
{
    public class RestaurantDbContextFactory(IDbContextFactory<RestaurantsContext> dbContextFactory, IMemoryCache cache, IWebHostEnvironment environment) : IRestaurantDbContextFactory
    {
        public PgDbContext CreateDbContextByConnectionString(string connectionString, bool readOnly = true, CancellationToken cancellationToken = default)
        {

            return GetDbContext(connectionString, readOnly);
        }

        public async Task<PgDbContext> CreateDbContextByUrlAsync(string restaurantUrl, bool readOnly = true,
            CancellationToken cancellationToken = default)
        {
            if (restaurantUrl is null)
            {
                throw new Exception("Restaurant URL is not set.");
            }

            var restaurant = await cache.GetOrCreateAsync(
                restaurantUrl,
                async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                    var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
                    return await dbContext.Restaurants
                        .AsNoTracking()
                        .Select(x => new { x.DomainName, x.ConnectionString })
                        .FirstOrDefaultAsync(
                            x => x.DomainName == restaurantUrl,
                            cancellationToken);
                }) ?? throw new Exception($"Restaurant '{restaurantUrl}' was not found.");
            var connectionString = restaurant.ConnectionString;

            return GetDbContext(connectionString, readOnly);
        }

        private PgDbContext GetDbContext(string connectionString, bool readOnly)
        {
            if (readOnly)
            {
                connectionString = connectionString.Replace("5434", "5433");
            }

            if (environment.IsDevelopment())
            {
                connectionString = connectionString.Replace("haproxy", "localhost");
            }
            var options = new DbContextOptionsBuilder<PgDbContext>()
                .UseNpgsql(connectionString, options =>
                {
                    options.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(2),
                        errorCodesToAdd: null);
                })
                .Options;


            return new PgDbContext(options);
        }
    }
}
