using Application.DTOs.AuthDTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NeonNovaApp.Controllers
{
    [Route("api/user")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly ICurrentUserService _currentUserService;

        public UserController(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        [HttpGet("current")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthenticationResponseDto))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<object>> GetCurrentUser()
        {
            var user = await _currentUserService.GetUser();
            if (user is null) return NotFound();
            return Ok(new
            {
                id = user.Id,
                email = user.Email,
            });
        }
    }
}