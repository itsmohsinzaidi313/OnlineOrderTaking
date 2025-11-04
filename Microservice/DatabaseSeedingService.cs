using Microsoft.EntityFrameworkCore;
using System.Data.Common;

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
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<DatabaseSeedingService>>();

        // Ensure we are connected to a writable primary before attempting schema changes.
        // In HA setups (Patroni, replicas) connections can land on a replica which will
        // reject CREATE TABLE with "cannot execute CREATE TABLE in a read-only transaction".
        await EnsureConnectedToPrimaryAsync(context, logger);
        await context.Database.EnsureCreatedAsync();
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

    private static async Task EnsureConnectedToPrimaryAsync(ProductsDbContext context, ILogger logger)
    {
        const int maxAttempts = 30; // ~30 * 1s = 30s max wait (exponential backoff applied)
        var attempt = 0;
        var delay = 1000;

        while (true)
        {
            attempt++;
            try
            {
                DbConnection? conn = context.Database.GetDbConnection();
                if (conn.State != System.Data.ConnectionState.Open)
                    await conn.OpenAsync();

                using var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT pg_is_in_recovery();"; // returns true on replicas
                var result = await cmd.ExecuteScalarAsync();

                if (result is bool inRecovery)
                {
                    if (!inRecovery)
                    {
                        logger.LogInformation("Connected to primary (writable). Proceeding with migrations/seeding.");
                        return;
                    }

                    logger.LogWarning("Connected to a replica (read-only). Will retry until primary is available. Attempt {Attempt}.", attempt);
                }
                else
                {
                    // If we can't determine the recovery state, conservatively proceed only after some retries
                    logger.LogWarning("pg_is_in_recovery() returned unexpected value ({Result}). Attempt {Attempt}.", result, attempt);
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error while checking primary/replica state. Attempt {Attempt}.", attempt);
            }

            if (attempt >= maxAttempts)
            {
                logger.LogError("Unable to detect a writable primary after {MaxAttempts} attempts. Aborting schema creation to avoid read-only errors.", maxAttempts);
                throw new InvalidOperationException("Writable primary not available for schema creation.");
            }

            await Task.Delay(delay);
            // simple exponential backoff, cap it
            delay = Math.Min(delay * 2, 5000);
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