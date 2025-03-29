using Microsoft.AspNetCore.Mvc;

namespace NeonNovaApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiagnosticController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { message = "API funcionando correctamente", timestamp = DateTime.UtcNow });
        }

        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new { status = "healthy" });
        }
    }
}