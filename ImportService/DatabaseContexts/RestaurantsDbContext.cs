using ImportService.Entities;
using Microsoft.EntityFrameworkCore;

namespace ImportService.Data
{
    public class RestaurantsDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<Restaurants> Restaurants => Set<Restaurants>();
        public DbSet<OrderTokens> OrderTokens => Set<OrderTokens>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Restaurants>()
                .ToTable("restaurants")
                .HasKey(x => x.Id);

            modelBuilder.Entity<OrderTokens>()
                .ToTable("order_tokens")
                .HasKey(x => x.Id);
        }
    }
}
