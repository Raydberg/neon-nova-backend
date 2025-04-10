using Application.DTOs.AuthDTOs;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NeonNovaApp.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthenticationResponseDto>> Register(CredentialsUserDto dto)
        {
            try
            {
                var response = await _authService.Register(dto);
                return Ok(response);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return ValidationProblem(ModelState);
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthenticationResponseDto>> Login(LoginRequestDto dto)
        {
            try
            {
                var response = await _authService.Login(dto);
                return Ok(response);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return ValidationProblem(ModelState);
            }
        }

        [HttpGet("removate-token")]
        public async Task<ActionResult<AuthenticationResponseDto>> RenovateToken()
        {
            try
            {
                var response = await _authService.RenovateToken();
                return Ok(response);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return ValidationProblem(ModelState);
            }
        }

        [HttpPost("set-admin")]
        public async Task<IActionResult> SetAdmin(EditClaimDto dto)
        {
            try
            {
                await _authService.SetAdmin(dto);
                return NoContent();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return ValidationProblem(ModelState);
            }
        }

        [HttpPost("remove-admin")]
        [Authorize(Policy = "isAdmin")]
        public async Task<IActionResult> RemoveAdmin(EditClaimDto dto)
        {
            try
            {
                await _authService.RemoveAdmin(dto);
                return NoContent();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return ValidationProblem(ModelState);
            }
        }
    }
}