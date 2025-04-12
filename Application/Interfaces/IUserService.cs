using Application.DTOs.UsersDTOs;

namespace Application.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<object> GetCurrentUserAsync();
    Task<UserDto> UpdateUserAsync(UserUpdateDto dto);
    Task<bool> DeleteUserAsync(string userId);
    Task<bool> DisableUserAsync(string userId);
    Task<bool> EnableUserAsync(string userId);
}