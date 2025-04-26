using Application.DTOs.UsersDTOs;

namespace Application.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<object> GetCurrentUserAsync();
    Task<UserDto> UpdateUserAsync(UserUpdateDto dto);
    Task<UserDto> GetUserByIdAsync(string userId);
    Task<bool> DeleteUserAsync(string userId);

    Task UpdateUserStatusAsync(string userId, bool isEnabled);
}