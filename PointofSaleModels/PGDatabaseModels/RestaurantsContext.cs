using Microsoft.EntityFrameworkCore;

namespace PointofSaleModels.PGDatabaseModels;

public partial class RestaurantsContext(DbContextOptions<RestaurantsContext> options) : DbContext(options)
{
    public virtual DbSet<Restaurant> Restaurants { get; set; }
    public virtual DbSet<OrderTokens> OrderTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Restaurant>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("restaurants");
        });

        modelBuilder.Entity<OrderTokens>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("order_tokens");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
