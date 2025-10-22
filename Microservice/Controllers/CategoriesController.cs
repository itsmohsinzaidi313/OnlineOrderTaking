using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Microservice.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CategoriesController(ProductsDbContext context, ILogger<CategoriesController> logger) : ControllerBase
{
    private readonly ProductsDbContext _context = context;
    private readonly ILogger<CategoriesController> _logger = logger;

    /// <summary>
    /// Get all categories
    /// </summary>
    /// <returns>A list of all categories with product counts</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetCategories()
    {
        try
        {
            var categories = await _context.Categories
                .OrderBy(c => c.Name)
                .Select(c => new { 
                    c.Id, 
                    c.Name, 
                    c.Description, 
                    ProductCount = c.Products.Count() 
                })
                .ToListAsync();

            return Ok(categories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving categories");
            return StatusCode(500, "An error occurred while retrieving categories");
        }
    }

    /// <summary>
    /// Get a specific category by ID
    /// </summary>
    /// <param name="id">The category ID</param>
    /// <returns>The category with the specified ID and its products</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetCategory(int id)
    {
        try
        {
            var category = await _context.Categories
                .Where(c => c.Id == id)
                .Select(c => new { 
                    c.Id, 
                    c.Name, 
                    c.Description,
                    Products = c.Products.Select(p => new { p.Id, p.Name, p.Price }).ToList()
                })
                .FirstOrDefaultAsync();

            if (category == null)
            {
                return NotFound($"Category with ID {id} not found");
            }

            return Ok(category);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving category with ID {CategoryId}", id);
            return StatusCode(500, "An error occurred while retrieving the category");
        }
    }

    /// <summary>
    /// Create a new category
    /// </summary>
    /// <param name="category">The category to create</param>
    /// <returns>The created category</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateCategory([FromBody] CreateCategoryRequest category)
    {
        try
        {
            // Validate the category
            if (string.IsNullOrWhiteSpace(category.Name))
            {
                return BadRequest("Category name is required");
            }

            // Check if category name already exists
            var nameExists = await _context.Categories.AnyAsync(c => c.Name == category.Name);
            if (nameExists)
            {
                return BadRequest("Category name already exists");
            }

            var newCategory = new Category
            {
                Name = category.Name,
                Description = category.Description ?? string.Empty,
                CreatedAt = DateTime.UtcNow
            };

            _context.Categories.Add(newCategory);
            await _context.SaveChangesAsync();

            // Return the created category
            var createdCategory = new
            {
                newCategory.Id,
                newCategory.Name,
                newCategory.Description,
                ProductCount = 0
            };

            return CreatedAtAction(nameof(GetCategory), new { id = newCategory.Id }, createdCategory);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category");
            return StatusCode(500, "An error occurred while creating the category");
        }
    }
}

/// <summary>
/// Request model for creating a category
/// </summary>
public class CreateCategoryRequest
{
    /// <summary>
    /// Category name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Category description (optional)
    /// </summary>
    public string? Description { get; set; }
}