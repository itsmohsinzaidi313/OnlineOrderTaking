using Microsoft.EntityFrameworkCore;

namespace Microservice;

public class DatabaseSeedingService
{
    public DatabaseSeedingService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    private readonly IServiceProvider _serviceProvider;

    public async Task SeedAsync()
    {
        // Create a scope here and resolve the DbContext and logger for the entire seeding run.
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ProductsDbContext>();
        await context.Database.EnsureCreatedAsync();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<DatabaseSeedingService>>();
        if (context.Departments.Count() > 0)
        {
            logger.LogInformation("Database already seeded. Skipping seeding process.");
            return;
        }
        try
        {
            logger.LogInformation("Starting database seeding...");

            // Seed mock departments if needed
            await SeedDepartmentsAsync(context, logger);

            // Seed additional categories if needed
            await SeedAdditionalCategoriesAsync(context, logger);

            // Seed additional products if needed
            await SeedAdditionalProductsAsync(context, logger);

            logger.LogInformation("Database seeding completed successfully!");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    private async Task SeedAdditionalCategoriesAsync(ProductsDbContext context, ILogger<DatabaseSeedingService> logger)
    {
        // Check if we have all our expected categories
        var existingCategoryNames = await context.Categories
            .Select(c => c.Name)
            .ToListAsync();

        var additionalCategories = new List<Category>();

        var expectedCategories = new[]
        {
            new { Name = "Accessories", Description = "Various accessories and add-ons" },
            new { Name = "Audio", Description = "Audio equipment and accessories" },
            new { Name = "Storage", Description = "Storage devices and solutions" }
        };

        foreach (var expectedCategory in expectedCategories)
        {
            if (!existingCategoryNames.Contains(expectedCategory.Name))
            {
                additionalCategories.Add(new Category
                {
                    Name = expectedCategory.Name,
                    Description = expectedCategory.Description
                });
            }
        }

        if (additionalCategories.Count > 0)
        {
            await context.Categories.AddRangeAsync(additionalCategories);
            await context.SaveChangesAsync();
            logger.LogInformation($"Added {additionalCategories.Count} additional categories.");
        }
    }

    private async Task SeedAdditionalProductsAsync(ProductsDbContext context, ILogger<DatabaseSeedingService> logger)
    {
        // Check current product count
        var productCount = await context.Products.CountAsync();

        if (productCount >= 15) // We already have enough products
        {
            logger.LogInformation($"Database already contains {productCount} products. Skipping additional seeding.");
            return;
        }

        // Get category IDs for assignment
        var categories = await context.Categories.ToDictionaryAsync(c => c.Name, c => c.Id);

        var additionalProducts = new List<Product>
        {
            new() { Name = "Gaming Chair", Price = 249.99m, CategoryId = categories.GetValueOrDefault("Gaming") },
            new() { Name = "Standing Desk", Price = 399.99m, CategoryId = categories.GetValueOrDefault("Office") },
            new() { Name = "External SSD 1TB", Price = 129.99m, CategoryId = categories.GetValueOrDefault("Storage") },
            new() { Name = "RGB Lighting Kit", Price = 49.99m, CategoryId = categories.GetValueOrDefault("Gaming") },
            new() { Name = "Noise Cancelling Headphones", Price = 299.99m, CategoryId = categories.GetValueOrDefault("Audio") },
            new() { Name = "Wireless Charger", Price = 39.99m, CategoryId = categories.GetValueOrDefault("Accessories") },
            new() { Name = "USB Microphone", Price = 89.99m, CategoryId = categories.GetValueOrDefault("Audio") },
            new() { Name = "Graphics Card", Price = 699.99m, CategoryId = categories.GetValueOrDefault("Computers") },
            new() { Name = "RAM 32GB Kit", Price = 199.99m, CategoryId = categories.GetValueOrDefault("Computers") },
            new() { Name = "CPU Cooler", Price = 79.99m, CategoryId = categories.GetValueOrDefault("Computers") }
        };

        // Only add products that don't already exist
        var existingProductNames = await context.Products
            .Select(p => p.Name)
            .ToListAsync();

        var newProducts = additionalProducts
            .Where(p => !existingProductNames.Contains(p.Name))
            .ToList();

        if (newProducts.Count > 0)
        {
            await context.Products.AddRangeAsync(newProducts);
            await context.SaveChangesAsync();
            logger.LogInformation($"Successfully seeded {newProducts.Count} additional products.");
        }
        else
        {
            logger.LogInformation("No additional products needed.");
        }
    }

    private async Task SeedDepartmentsAsync(ProductsDbContext context, ILogger<DatabaseSeedingService> logger)
    {
        // Check if we have expected departments
        var existingDepartmentNames = await context.Departments
            .Select(d => d.Name)
            .ToListAsync();

        var expectedDepartments = new[]
        {
            new { Name = "Marketing", Description = "Marketing and communications" },
            new { Name = "Support", Description = "Customer support and success" },
            new { Name = "Finance", Description = "Accounting and finance" }
        };

        var additionalDepartments = new List<Department>();

        foreach (var expected in expectedDepartments)
        {
            if (!existingDepartmentNames.Contains(expected.Name))
            {
                additionalDepartments.Add(new Department
                {
                    Name = expected.Name,
                    Description = expected.Description
                });
            }
        }

        if (additionalDepartments.Count > 0)
        {
            await context.Departments.AddRangeAsync(additionalDepartments);
            await context.SaveChangesAsync();
            logger.LogInformation($"Added {additionalDepartments.Count} additional departments.");
        }
        else
        {
            logger.LogInformation("No additional departments needed.");
        }
    }
}