using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NeonNovaApp.Controllers;

[Route("api/proxy")]
[ApiController]
public class ImageProxyController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ImageProxyController> _logger;

    public ImageProxyController(IHttpClientFactory httpClientFactory, ILogger<ImageProxyController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    [HttpGet("image")]
    [AllowAnonymous]
    [ResponseCache(Duration = 86400)]
    public async Task<IActionResult> ProxyImage([FromQuery] string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            return BadRequest("URL de imagen no proporcionada");
        }

        try
        {
            Uri uri = new Uri(url);
            string host = uri.Host.ToLower();

            string[] allowedDomains = new[] {
            "lh3.googleusercontent.com",
            "googleusercontent.com",
            "ggpht.com",
            "googleapis.com"
        };

            bool isDomainAllowed = allowedDomains.Any(domain => host.EndsWith(domain));

            if (!isDomainAllowed)
            {
                return BadRequest("Dominio de imagen no permitido");
            }

            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(10);

            // Añadir un header User-Agent para evitar restricciones
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 NeonNova/1.0");

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Error al obtener imagen: {StatusCode} - {Url}", response.StatusCode, url);
                return StatusCode((int)response.StatusCode, "Error al obtener la imagen");
            }

            var contentType = response.Content.Headers.ContentType?.ToString() ?? "image/jpeg";
            var imageBytes = await response.Content.ReadAsByteArrayAsync();

            Response.Headers.Add("Cache-Control", "public, max-age=86400");
            Response.Headers.Add("Pragma", "cache");
            Response.Headers.Add("Expires", DateTime.UtcNow.AddDays(1).ToString("R"));

            return File(imageBytes, contentType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener imagen de {Url}", url);
            return StatusCode(500, "Error al procesar la imagen");
        }
    }
}