using Microsoft.EntityFrameworkCore;

namespace Microservice;

public class ProductsDbContext(DbContextOptions<ProductsDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Department> Departments => Set<Department>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Category entity
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.Description)
                .HasMaxLength(500);
            
            // Create index on Name for better query performance
            entity.HasIndex(e => e.Name).IsUnique();
            
            // Configure optional relationship to Department
            entity.HasOne(e => e.Department)
                .WithMany(d => d.Categories)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configure Product entity
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(e => e.Price)
                .HasPrecision(18, 2);
            
            // Create index on Name for better query performance
            entity.HasIndex(e => e.Name);
            
            // Configure relationship with Category
            entity.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Seed initial data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed Categories
        // Note: DepartmentIds reference seeded Departments (1:Sales, 2:Engineering, 3:HR)
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Electronics", Description = "Electronic devices and gadgets", DepartmentId = 2 },
            new Category { Id = 2, Name = "Computers", Description = "Computer hardware and accessories", DepartmentId = 2 },
            new Category { Id = 3, Name = "Mobile", Description = "Mobile devices and accessories", DepartmentId = 2 },
            new Category { Id = 4, Name = "Gaming", Description = "Gaming equipment and accessories", DepartmentId = 2 },
            new Category { Id = 5, Name = "Office", Description = "Office furniture and supplies", DepartmentId = 1 }
        );

        // Seed Products with Categories
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Laptop Pro", Price = 1299.99m, CategoryId = 2 },
            new Product { Id = 2, Name = "Wireless Mouse", Price = 29.99m, CategoryId = 2 },
            new Product { Id = 3, Name = "Mechanical Keyboard", Price = 149.99m, CategoryId = 2 },
            new Product { Id = 4, Name = "4K Monitor", Price = 399.99m, CategoryId = 2 },
            new Product { Id = 5, Name = "USB-C Hub", Price = 79.99m, CategoryId = 2 },
            new Product { Id = 6, Name = "Webcam HD", Price = 89.99m, CategoryId = 1 },
            new Product { Id = 7, Name = "Bluetooth Headphones", Price = 199.99m, CategoryId = 1 },
            new Product { Id = 8, Name = "Smartphone", Price = 799.99m, CategoryId = 3 },
            new Product { Id = 9, Name = "Tablet", Price = 449.99m, CategoryId = 3 },
            new Product { Id = 10, Name = "Smart Watch", Price = 299.99m, CategoryId = 3 }
        );

        // Seed Departments
        modelBuilder.Entity<Department>().HasData(
            new Department { Id = 1, Name = "Sales", Description = "Handles sales and client relations" },
            new Department { Id = 2, Name = "Engineering", Description = "Develops products and features" },
            new Department { Id = 3, Name = "HR", Description = "Human resources and recruitment" }
        );
    }
}
