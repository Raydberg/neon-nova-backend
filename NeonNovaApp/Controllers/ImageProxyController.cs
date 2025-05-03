using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;

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
            Uri uri;
            try
            {
                uri = new Uri(url);
            }
            catch (UriFormatException ex)
            {
                _logger.LogWarning(ex, "URL de imagen inválida: {Url}", url);
                return BadRequest("URL de imagen inválida");
            }

            string host = uri.Host.ToLower();
            if (host.Contains("googleusercontent.com") || host.Contains("ggpht.com"))
            {
                _logger.LogInformation("Redirigiendo directamente a imagen de Google: {Url}", url);
                return Redirect(url);
            }
            string[] allowedDomains = new[] {
                "lh3.googleusercontent.com",
                "googleusercontent.com",
                "ggpht.com",
                "googleapis.com"
            };

            bool isDomainAllowed = allowedDomains.Any(domain => host.EndsWith(domain));

            if (!isDomainAllowed)
            {
                _logger.LogWarning("Dominio no permitido: {Host}", host);
                return BadRequest("Dominio de imagen no permitido");
            }

            // Usar un cliente HTTP configurado específicamente para Google
            var client = _httpClientFactory.CreateClient("GoogleImageProxy");

            // Configuración específica para este request
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.0.0 Safari/537.36");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("image/*"));
            client.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "no-cors");
            client.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "image");
            client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9,es;q=0.8");
            client.DefaultRequestHeaders.Referrer = new Uri("https://accounts.google.com/");

            // Si es necesario, también puedes añadir cookies o encabezados de autorización aquí

            // Aumentar el tiempo de espera
            client.Timeout = TimeSpan.FromSeconds(15);

            using var request = new HttpRequestMessage(HttpMethod.Get, url);

            // Intentar obtener la imagen
            var response = await client.SendAsync(request);

            // Si falla con 403, intentar con un enfoque alternativo
            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                _logger.LogWarning("Acceso prohibido, intentando con enfoque alternativo para: {Url}", url);

                // En este punto, podrías considerar utilizar una imagen de avatar genérica
                // o intentar con un User-Agent diferente
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (iPhone; CPU iPhone OS 15_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/15.0 Mobile/15E148 Safari/604.1");

                using var secondRequest = new HttpRequestMessage(HttpMethod.Get, url);
                response = await client.SendAsync(secondRequest);
            }

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Error al obtener imagen: {StatusCode} - {Url}", response.StatusCode, url);

                // Devolver una imagen de avatar predeterminada en lugar de un error
                return File(System.IO.File.ReadAllBytes("wwwroot/default-avatar.png"), "image/png");
            }

            var contentType = response.Content.Headers.ContentType?.ToString() ?? "image/jpeg";
            var imageBytes = await response.Content.ReadAsByteArrayAsync();

            if (imageBytes == null || imageBytes.Length == 0)
            {
                _logger.LogWarning("Imagen vacía recibida de: {Url}", url);
                return File(System.IO.File.ReadAllBytes("wwwroot/default-avatar.png"), "image/png");
            }

            Response.Headers.Add("Cache-Control", "public, max-age=86400");
            Response.Headers.Add("Pragma", "cache");
            Response.Headers.Add("Expires", DateTime.UtcNow.AddDays(1).ToString("R"));

            return File(imageBytes, contentType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar imagen de {Url}: {Message}", url, ex.Message);

            // Devolver una imagen predeterminada en caso de error
            return File(System.IO.File.ReadAllBytes("wwwroot/default-avatar.png"), "image/png");
        }
    }
}