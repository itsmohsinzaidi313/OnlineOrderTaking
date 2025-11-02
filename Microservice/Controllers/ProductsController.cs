using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Microservice.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProductsController(ProductsDbContext context, ILogger<ProductsController> logger) : ControllerBase
{
    private readonly ProductsDbContext _context = context;
    private readonly ILogger<ProductsController> _logger = logger;

    /// <summary>
    /// Get all products
    /// </summary>
    /// <returns>A list of all products with their categories</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetProducts()
    {
        try
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .OrderBy(p => p.Name)
                .Select(p => new {
                    p.Id,
                    p.Name,
                    p.Price,
                    p.CreatedAt,
                    p.CategoryId,
                    Category = p.Category != null ? new { 
                        p.Category.Id, 
                        p.Category.Name, 
                        p.Category.Description 
                    } : null
                })
                .ToListAsync();

            return Ok(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving products");
            return StatusCode(500, "An error occurred while retrieving products");
        }
    }

    /// <summary>
    /// Get a specific product by ID
    /// </summary>
    /// <param name="id">The product ID</param>
    /// <returns>The product with the specified ID</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetProduct(int id)
    {
        try
        {
            var product = await _context.Products
                .Where(p => p.Id == id)
                .Select(p => new {
                    p.Id,
                    p.Name,
                    p.Price,
                    p.CreatedAt,
                    p.CategoryId,
                    Category = p.Category != null ? new { 
                        p.Category.Id, 
                        p.Category.Name, 
                        p.Category.Description 
                    } : null
                })
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return NotFound($"Product with ID {id} not found");
            }

            return Ok(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving product with ID {ProductId}", id);
            return StatusCode(500, "An error occurred while retrieving the product");
        }
    }

    /// <summary>
    /// Get all products in a specific category
    /// </summary>
    /// <param name="categoryId">The category ID</param>
    /// <returns>A list of products in the specified category</returns>
    [HttpGet("category/{categoryId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetProductsByCategory(int categoryId)
    {
        try
        {
            var products = await _context.Products
                .Where(p => p.CategoryId == categoryId)
                .OrderBy(p => p.Name)
                .Select(p => new {
                    p.Id,
                    p.Name,
                    p.Price,
                    p.CreatedAt,
                    p.CategoryId,
                    Category = p.Category != null ? new { 
                        p.Category.Id, 
                        p.Category.Name, 
                        p.Category.Description 
                    } : null
                })
                .ToListAsync();

            return Ok(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving products for category {CategoryId}", categoryId);
            return StatusCode(500, "An error occurred while retrieving products by category");
        }
    }

    /// <summary>
    /// Create a new product
    /// </summary>
    /// <param name="product">The product to create</param>
    /// <returns>The created product</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateProduct([FromBody] CreateProductRequest product)
    {
        try
        {
            // Validate the product
            if (string.IsNullOrWhiteSpace(product.Name))
            {
                return BadRequest("Product name is required");
            }

            if (product.Price <= 0)
            {
                return BadRequest("Product price must be greater than 0");
            }

            // Check if category exists if provided
            if (product.CategoryId.HasValue)
            {
                var categoryExists = await _context.Categories.AnyAsync(c => c.Id == product.CategoryId.Value);
                if (!categoryExists)
                {
                    return BadRequest("Category does not exist");
                }
            }

            var newProduct = new Product
            {
                Name = product.Name,
                Price = product.Price,
                CategoryId = product.CategoryId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Products.Add(newProduct);
            await _context.SaveChangesAsync();

            // Return the created product with category info
            var createdProduct = await _context.Products
                .Where(p => p.Id == newProduct.Id)
                .Select(p => new {
                    p.Id,
                    p.Name,
                    p.Price,
                    p.CreatedAt,
                    p.CategoryId,
                    Category = p.Category != null ? new { 
                        p.Category.Id, 
                        p.Category.Name, 
                        p.Category.Description 
                    } : null
                })
                .FirstOrDefaultAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = newProduct.Id }, createdProduct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product");
            return StatusCode(500, "An error occurred while creating the product");
        }
    }
}

/// <summary>
/// Request model for creating a product
/// </summary>
public class CreateProductRequest
{
    /// <summary>
    /// Product name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Product price
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Category ID (optional)
    /// </summary>
    public int? CategoryId { get; set; }
}