using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.DTOs.AuthDTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.IdentityModel.Tokens;

namespace NeonNovaApp.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<Users> _userManager;
        private readonly SignInManager<Users> _signInManager;
        private readonly ICurrentUserService _userService;

        public AuthController(UserManager<Users> userManager,
            SignInManager<Users> signInManager, ICurrentUserService userService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userService = userService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthenticationResponseDto>> Register(CredentialsUserDto dto)
        {
            var user = new Users
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                UserName = dto.Email,
                PhoneNumber = dto.Phone,
                //Mas adelante poner verificaicon de Email
                EmailConfirmed = true
            };
            var result = await _userManager.CreateAsync(user, dto.Password!);
            if (result.Succeeded)
            {
                var loginDto = new LoginRequestDto
                {
                    Email = dto.Email,
                    Password = dto.Password
                };
                var responseAuthentication = await CreateToken(loginDto);
                return responseAuthentication;
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return ValidationProblem();
            }
        }

        [HttpPost]
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthenticationResponseDto>> Login(LoginRequestDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user is null) return ReturnLoginFalied();
            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password!, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                return await CreateToken(dto);
            }
            else
            {
                return ReturnLoginFalied();
            }
        }

        private ActionResult ReturnLoginFalied()
        {
            ModelState.AddModelError(string.Empty, "Login Incorrecto");
            return ValidationProblem();
        }

        [HttpGet("removate-token")]
        public async Task<ActionResult<AuthenticationResponseDto>> RenovateToken()
        {
            var user = await _userService.GetUser();
            if (user is null) return NotFound();
            var credentialUserDto = new LoginRequestDto
            {
                Email = user.Email!,
                Password = user.PasswordHash
            };
            var responseAuthentication = await CreateToken(credentialUserDto!);
            return responseAuthentication;
        }

        [HttpPost("set-admin")]
        // [Authorize(Policy = "isAdmin")]
        public async Task<ActionResult> SetAdmin(EditClaimDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user is null) return NotFound();
            await _userManager.AddClaimAsync(user, new Claim("isAdmin", "true"));
            return NoContent();
        }

        [HttpPost("remove-admin")]
        [Authorize(Policy = "isAdmin")]
        public async Task<ActionResult> RemoveAdmin(EditClaimDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user is null) return NotFound();
            await _userManager.RemoveClaimAsync(user, new Claim("isAdmin", "true"));
            return NoContent();
        }

        private async Task<AuthenticationResponseDto> CreateToken(LoginRequestDto dto)
        {
            var claims = new List<Claim>
            {
                new Claim("email", dto.Email)
            };
            //Ir a la db a buscar los claims
            var user = await _userManager.FindByEmailAsync(dto.Email);
            var claimsDB = await _userManager.GetClaimsAsync(user!);
            claims.AddRange(claimsDB);
            var keyJwt = Environment.GetEnvironmentVariable("key_jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyJwt!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expired = DateTime.UtcNow.AddHours(3);
            var tokenSecurity = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expired,
                signingCredentials: credentials);
            var token = new JwtSecurityTokenHandler().WriteToken(tokenSecurity);
            return new AuthenticationResponseDto
            {
                Token = token,
                Expired = expired
            };
        }
    }
}