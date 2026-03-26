using Microsoft.AspNetCore.Mvc;

namespace workswap.Controllers;

/// <summary>
/// Simple health check endpoint for monitoring and deployment verification.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    /// <summary>
    /// Returns a 200 OK response with a simple health status.
    /// </summary>
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            status = "healthy",
            timestamp = DateTime.UtcNow
        });
    }
}
