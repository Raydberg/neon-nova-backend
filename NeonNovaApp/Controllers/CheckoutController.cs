using Application.DTOs.CheckoutDTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace NeonNovaApp.Controllers;

[ApiController]
[Route("api/checkout")]
public class CheckoutController : ControllerBase
{
    private readonly ICheckoutService _checkoutService;
    private readonly ILogger<CheckoutController> _logger;

    public CheckoutController(
        ICheckoutService checkoutService, 
        ILogger<CheckoutController> logger)
    {
        _checkoutService = checkoutService;
        _logger = logger;
    }

    [HttpPost("personal-info")]
    public async Task<IActionResult> PersonalInfo([FromBody] PersonalInfoDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            _logger.LogInformation("📦 Dirección recibida: {Street}, {City}, {Postal}", 
                dto.Address.Street, dto.Address.City, dto.Address.PostalCode);

            var addressId = await _checkoutService.SavePersonalInfoAsync(dto);

            return Ok(new { message = "Dirección de envío guardada correctamente", addressId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al guardar información personal");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost("payment-method")]
    public async Task<IActionResult> SetPaymentMethod([FromBody] PaymentMethodDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        
        try
        {
            var paymentMethodId = await _checkoutService.CreatePaymentMethodAsync(dto);
            return Ok(new { paymentMethodId });
        }
        catch (ApplicationException ae)
        {
            return BadRequest(new { message = ae.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear método de pago");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost("create-checkout-session")]
    public async Task<IActionResult> CreateCheckoutSession([FromBody] CheckoutSessionRequestDto req)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        
        try
        {
            var url = await _checkoutService.CreateCheckoutSessionAsync(req);
            return Ok(new { url });
        }
        catch (ApplicationException ae)
        {
            return BadRequest(new { message = ae.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ ERROR EN CREAR SESIÓN STRIPE");
            return StatusCode(500, "Error al crear la sesión de pago.");
        }
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> StripeWebhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        
        try
        {
            var success = await _checkoutService.ProcessWebhookAsync(json, Request.Headers["Stripe-Signature"]);
            return success ? Ok() : BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error en webhook Stripe");
            return BadRequest();
        }
    }

    [HttpGet("checkout/session/{id}")]
    public async Task<IActionResult> GetSessionDetails(string id)
    {
        var session = await _checkoutService.GetSessionDetailsAsync(id);
        return Ok(session);
    }

    [HttpPost("cart/save")]
    public async Task<IActionResult> SaveCart([FromBody] SaveCartDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            await _checkoutService.SaveCartAsync(dto);
            return Ok();
        }
        catch (ApplicationException ae)
        {
            return BadRequest(new { message = ae.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al guardar el carrito");
            return StatusCode(500, "Error al guardar el carrito.");
        }
    }
}