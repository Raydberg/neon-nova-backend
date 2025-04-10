using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.DTOs.AuthDTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly SignInManager<Users> _signInManager;
        private readonly ICurrentUserService _currentUserService;

        public AuthService(IAuthRepository authRepository,
            SignInManager<Users> signInManager,
            ICurrentUserService currentUserService)
        {
            _authRepository = authRepository;
            _signInManager = signInManager;
            _currentUserService = currentUserService;
        }

        public async Task<AuthenticationResponseDto> Register(CredentialsUserDto dto)
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

            var result = await _authRepository.CreateUserAsync(user, dto.Password!);
            if (result.Succeeded)
            {
                var loginDto = new LoginRequestDto
                {
                    Email = dto.Email,
                    Password = dto.Password
                };
                return await CreateToken(loginDto);
            }
            else
            {
                // Aquí puedes manejar la validación según tus necesidades
                throw new Exception("Error al registrar el usuario");
            }
        }

        public async Task<AuthenticationResponseDto> Login(LoginRequestDto dto)
        {
            var user = await _authRepository.FindUserByEmailAsync(dto.Email);
            if (user is null) throw new Exception("Login Incorrecto");

            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password!, false);
            if (result.Succeeded)
            {
                return await CreateToken(dto);
            }
            else
            {
                throw new Exception("Login Incorrecto");
            }
        }

        public async Task<AuthenticationResponseDto> RenovateToken()
        {
            var user = await _currentUserService.GetUser();
            if (user is null) throw new Exception("Usuario no encontrado");

            var loginDto = new LoginRequestDto
            {
                Email = user.Email!,
                // Nota: Generalmente, no se usa el PasswordHash para renovar tokens.
                // Aquí se asume que se utiliza el flujo adecuado para renovar.
                Password = string.Empty 
            };

            return await CreateToken(loginDto);
        }

        public async Task SetAdmin(EditClaimDto dto)
        {
            var user = await _authRepository.FindUserByEmailAsync(dto.Email);
            if (user is null) throw new Exception("Usuario no encontrado");

            await _authRepository.AddClaimAsync(user, new Claim("isAdmin", "true"));
        }

        public async Task RemoveAdmin(EditClaimDto dto)
        {
            var user = await _authRepository.FindUserByEmailAsync(dto.Email);
            if (user is null) throw new Exception("Usuario no encontrado");

            await _authRepository.RemoveClaimAsync(user, new Claim("isAdmin", "true"));
        }

        private async Task<AuthenticationResponseDto> CreateToken(LoginRequestDto dto)
        {
            var claims = new List<Claim>
            {
                new Claim("email", dto.Email)
            };
            var user = await _authRepository.FindUserByEmailAsync(dto.Email);
            var claimsDB = await _authRepository.GetUserClaimsAsync(user!);
            claims.AddRange(claimsDB);
            var keyJwt = Environment.GetEnvironmentVariable("key_jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyJwt!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddHours(3);
            var tokenSecurity = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expiration,
                signingCredentials: credentials);
            var token = new JwtSecurityTokenHandler().WriteToken(tokenSecurity);
            return new AuthenticationResponseDto
            {
                Token = token,
                Expired = expiration
            };
        }
    }
}
