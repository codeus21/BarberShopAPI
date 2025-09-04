using Microsoft.AspNetCore.Mvc;

namespace BarberShopAPI.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
        }
    }

    // Simple health check for Railway (no tenant required)
    [ApiController]
    [Route("")]
    public class RailwayHealthController : ControllerBase
    {
        [HttpGet("health")]
        public IActionResult Get()
        {
            return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
        }
    }
}