using Application.DTOs.UsersDTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NeonNovaApp.Controllers;

[Route("api/user")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    // [Authorize(Policy = "isAdmin")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpGet("current")]
    // [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<object>> GetCurrentUser()
    {
        var user = await _userService.GetCurrentUserAsync();
        if (user is null) return NotFound();

        return Ok(user);
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<UserDto>> GetUserById(string userId)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(userId);
            return Ok(user);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al obtener usuario: {ex.Message}");
        }
    }

    [HttpPut]
    [Authorize]
    public async Task<IActionResult> UpdateUser(UserUpdateDto dto)
    {
        try
        {
            var userUpdate = await _userService.UpdateUserAsync(dto);
            return Ok(userUpdate);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al actualizar usuario: {ex.Message}");
        }
    }

    [HttpDelete("{userId}")]
    // [Authorize(Policy = "isAdmin")]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        try
        {
            await _userService.DeleteUserAsync(userId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al eliminar usuario: {ex.Message}");
        }
    }

    [HttpPut("{userId}/disable")]
    // [Authorize(Policy = "isAdmin")]
    public async Task<IActionResult> DisableUser(string userId)
    {
        try
        {
            await _userService.DisableUserAsync(userId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al deshabilitar usuario: {ex.Message}");
        }
    }

    [HttpPut("{userId}/enable")]
    // [Authorize(Policy = "isAdmin")]
    public async Task<IActionResult> EnableUser(string userId)
    {
        try
        {
            await _userService.EnableUserAsync(userId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al habilitar usuario: {ex.Message}");
        }
    }
}