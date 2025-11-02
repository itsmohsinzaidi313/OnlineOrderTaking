using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Microservice.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class DepartmentsController(ProductsDbContext context, ILogger<DepartmentsController> logger) : ControllerBase
{
    private readonly ProductsDbContext _context = context;
    private readonly ILogger<DepartmentsController> _logger = logger;

    /// <summary>
    /// Get all departments
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetDepartments()
    {
        try
        {
            var departments = await _context.Departments
                .OrderBy(d => d.Name)
                .Select(d => new { d.Id, d.Name, d.Description })
                .ToListAsync();

            return Ok(departments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving departments");
            return StatusCode(500, "An error occurred while retrieving departments");
        }
    }

    /// <summary>
    /// Get a specific department by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetDepartment(int id)
    {
        try
        {
            var department = await _context.Departments
                .Where(d => d.Id == id)
                .Select(d => new { d.Id, d.Name, d.Description })
                .FirstOrDefaultAsync();

            if (department == null)
            {
                return NotFound($"Department with ID {id} not found");
            }

            return Ok(department);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving department with ID {DepartmentId}", id);
            return StatusCode(500, "An error occurred while retrieving the department");
        }
    }
}
