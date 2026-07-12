using PointofSaleModels.PGDatabaseModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace PointofSaleModels.Services
{
    public interface IRestaurantDbContextFactory
    {
        Task<PgDbContext> CreateDbContextByUrlAsync(string restaurantUrl, bool readOnly = true, CancellationToken cancellationToken = default);
        PgDbContext CreateDbContextByConnectionString(string connectionString, bool readOnly = true, CancellationToken cancellationToken = default);
    }
}
