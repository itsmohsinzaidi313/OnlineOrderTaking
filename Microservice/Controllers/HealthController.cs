using Microsoft.AspNetCore.Mvc;

namespace Microservice.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class HealthController : ControllerBase
{
    /// <summary>
    /// Health check endpoint
    /// </summary>
    /// <returns>Service status</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<object> GetHealth()
    {
        return Ok(new { 
            Status = "Healthy", 
            Message = "Microservice is running ✅",
            Timestamp = DateTime.UtcNow
        });
    }
}