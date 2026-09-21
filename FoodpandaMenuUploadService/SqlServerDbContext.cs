using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodpandaMenuUploadService
{
    public class SqlServerDbContext(DbContextOptions<SqlServerDbContext> options) : DbContext(options)
    {
        public DbSet<SetupCompany> SetupCompanies => Set<SetupCompany>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SetupCompany>()
                .ToTable("SetupCompany")
                .HasKey(x => x.Id);
        }
    }
    public class SetupCompany
    {
        [Column("CompanyId")]
        public int Id { get; set; }
        [Column("WebsiteUrl")]
        public string? Url { get; set; }
        [Column("IsEnable")]
        public bool? Enabled { get; set; }
    }
}
