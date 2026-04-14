using Microsoft.EntityFrameworkCore;
using PointofSaleModels.PGDatabaseModels;

namespace PointofSaleModels.Services
{
    public interface IConnectionStringResolver
    {
        Task<string> ResolveAsync(string domain);
        PgDbContext GetReadOnlyDbContext(string connectionString);
        PgDbContext GetWriteDbContext(string connectionString);
        Task<PgDbContext> ResolveAndGetReadOnlyDbContextAsync(string domain);
        Task<PgDbContext> ResolveAndGetWriteDbContextAsync(string domain);
        RestaurantsContext GetRestaurantsContext();
    }

    public class ConnectionStringResolver(IDbContextFactory<RestaurantsContext> dbContextFactory) : IConnectionStringResolver
    {
        public RestaurantsContext GetRestaurantsContext() => dbContextFactory.CreateDbContext();
        public async Task<string> ResolveAsync(string domain)
        {
            await using var context = await dbContextFactory.CreateDbContextAsync();
            var restaurant = await context.Restaurants.FirstOrDefaultAsync(r => r.DomainName == domain);
            return restaurant == null ? throw new Exception("Restaurant not found") : restaurant.ConnectionString;
        }

        public PgDbContext GetReadOnlyDbContext(string connectionString)
        {
            return GetDbContext(connectionString);
        }

        public PgDbContext GetWriteDbContext(string connectionString)
        {
            return GetDbContext(connectionString.Replace("5434", "5433"));
        }

        private PgDbContext GetDbContext(string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder<PgDbContext>();
            optionsBuilder.UseNpgsql(connectionString, options =>
            {
                options.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorCodesToAdd: null);
            });
            return new PgDbContext(optionsBuilder.Options);
        }

        public async Task<PgDbContext> ResolveAndGetReadOnlyDbContextAsync(string domain)
        {
            var connectionString = await ResolveAsync(domain);
            return GetReadOnlyDbContext(connectionString);
        }

        public async Task<PgDbContext> ResolveAndGetWriteDbContextAsync(string domain)
        {
            var connectionString = await ResolveAsync(domain);
            return GetWriteDbContext(connectionString);
        }
    }
}
