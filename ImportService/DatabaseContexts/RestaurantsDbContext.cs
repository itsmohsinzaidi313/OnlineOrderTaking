using ImportService.Entities;
using Microsoft.EntityFrameworkCore;

namespace ImportService.Data
{
    public class RestaurantsDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<Restaurants> Restaurants => Set<Restaurants>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Restaurants>()
                .ToTable("restaurants")
                .HasKey(x => x.Id);
        }
    }
}
