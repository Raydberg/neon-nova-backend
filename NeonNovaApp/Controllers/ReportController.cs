using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace NeonNovaApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly IGenerateInvoiceCommand _cmd;

        public ReportController(IGenerateInvoiceCommand cmd)
            => _cmd = cmd;

        [HttpGet("generate")]
        public async Task<IActionResult> Generate()
        {
            byte[] pdf = await _cmd.ExecuteAsync();
            return File(pdf, "application/pdf", "ReporteFacturas.pdf");
        }
    }
}