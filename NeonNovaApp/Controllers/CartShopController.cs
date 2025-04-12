using Application.DTOs.CartShopDTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NeonNovaApp.Controllers;

[Route("api/cart")]
[ApiController]
[Authorize]
public class CartShopController : ControllerBase
{
    private readonly ICartShopService _cartShopService;

    public CartShopController(ICartShopService cartShopService)
    {
        _cartShopService = cartShopService;
    }

    [HttpGet]
    public async Task<ActionResult<CartShopDto>> GetCurrentCart()
    {
        try
        {
            var cart = await _cartShopService.GetCurrentCartAsync();
            return Ok(cart);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<CartShopDto>> AddToCart([FromBody] AddToCartShopDto dto)
    {
        try
        {
            var cart = await _cartShopService.AddToCartAsync(dto);
            return Ok(cart);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut]
    public async Task<ActionResult<CartShopDto>> UpdateCartItem([FromBody] UpdateCartShopItemDto dto)
    {
        try
        {
            var cart = await _cartShopService.UpdateCartItemAsync(dto);
            return Ok(cart);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<CartShopDto>> RemoveCartItem(int id)
    {
        try
        {
            var cart = await _cartShopService.RemoveCartItemAsync(id);
            return Ok(cart);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("clear")]
    public async Task<ActionResult<bool>> ClearCart()
    {
        try
        {
            var result = await _cartShopService.ClearCartAsync();
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("complete")]
    public async Task<ActionResult<bool>> CompleteCart()
    {
        try
        {
            var result = await _cartShopService.CompleteCartAsync();
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<CartShopDto>> RefreshCart()
    {
        try
        {
            var cart = await _cartShopService.RefreshCartAsync();
            return Ok(cart);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}