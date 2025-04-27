using Microsoft.AspNetCore.Mvc;
using Intrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace NeonNovaApp.Controllers
{
    [ApiController]
    [Route("api/diagnostic")]
    public class DiagnosticController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public DiagnosticController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

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

        [HttpGet("database")]
        public async Task<IActionResult> TestDatabase()
        {
            try
            {
                bool canConnect = await _dbContext.Database.CanConnectAsync();

                if (canConnect)
                {
                    return Ok(new
                    {
                        message = "Conexión a la base de datos exitosa",
                        timestamp = DateTime.UtcNow
                    });
                }
                else
                {
                    return StatusCode(500, new
                    {
                        message = "No se pudo conectar a la base de datos",
                        timestamp = DateTime.UtcNow
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error al conectar con la base de datos",
                    error = ex.Message,
                    innerError = ex.InnerException?.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }
    }
}