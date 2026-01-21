using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PointofSaleModels.PGDatabaseModels;

public partial class RestaurantsContext : DbContext
{
    public RestaurantsContext()
    {
    }

    public RestaurantsContext(DbContextOptions<RestaurantsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Restaurant> Restaurants { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Restaurant>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("restaurants");

            entity.Property(e => e.ConnectionString)
                .HasMaxLength(128)
                .HasColumnName("connection_string");
            entity.Property(e => e.DomainName)
                .HasMaxLength(128)
                .HasColumnName("domain_name");
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(128)
                .HasColumnName("name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
