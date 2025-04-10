using Application.DTOs.AuthDTOs;
using Domain.Entities;

namespace Application.Services;

public interface IAuthService
{
    Task<AuthenticationResponseDto> Register(CredentialsUserDto dto);
    Task<AuthenticationResponseDto> Login(LoginRequestDto dto);
    Task<AuthenticationResponseDto> RenovateToken();
    Task SetAdmin(EditClaimDto dto);
    Task RemoveAdmin(EditClaimDto dto);
}