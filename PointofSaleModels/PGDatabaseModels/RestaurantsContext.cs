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
                .ToTable("restaurants")
                .HasKey(x => x.Id);
        });

        modelBuilder.Entity<OrderTokens>(entity =>
        {
            entity
                .ToTable("order_tokens")
                .HasKey(x => x.Id);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
