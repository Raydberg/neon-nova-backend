using System.Security.Claims;
using Application.DTOs.AuthDTOs;
namespace Application.Interfaces;

public interface IAuthService
{
    Task<AuthenticationResponseDto> Register(CredentialsUserDto dto);
    Task<AuthenticationResponseDto> Login(LoginRequestDto dto);
    Task<AuthenticationResponseDto> RenovateToken();
    Task SetAdmin(EditClaimDto dto);
    Task RemoveAdmin(EditClaimDto dto);
    Task<AuthenticationResponseDto> LoginWithGoogleAsync(ClaimsPrincipal claimsPrincipal);
    
}