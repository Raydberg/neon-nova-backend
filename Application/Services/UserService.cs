using Application.DTOs.UsersDTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly UserManager<Users> _userManager;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public UserService(IUserRepository userRepository, UserManager<Users> userManager, IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _userManager = userManager;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllUsersAsync();
        return _mapper.Map<IEnumerable<UserDto>>(users);
    }

    public async Task<object> GetCurrentUserAsync()
    {
        var user = await _currentUserService.GetUser();
        if (user is null) return null;
        var claims = await _userManager.GetClaimsAsync(user);
        var isAdmin = claims.Any(c => c.Type == "isAdmin" && c.Value == "true");
        var isUser = claims.Any(c => c.Type == "isUser" && c.Value == "true");

        return new
        {
            id = user.Id,
            email = user.Email,
            name = $"{user.FirstName} {user.LastName}",
            firstName = user.FirstName ?? string.Empty,
            lastName = user.LastName ?? string.Empty,
            phone = user.PhoneNumber,
            permition = new
            {
                ADMIN = isAdmin,
                USER = isUser
            }
        };
    }

    public async Task<UserDto> UpdateUserAsync(UserUpdateDto dto)
    {
        var user = await _currentUserService.GetUser();
        if (user is null) throw new KeyNotFoundException("Usuario no encontrado");

        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;

        var updatedUser = await _userRepository.UpdateUserAsync(user);
        return _mapper.Map<UserDto>(updatedUser);
    }

    public async Task<bool> DeleteUserAsync(string userId)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user is null) throw new KeyNotFoundException("Usuario no encontrado");

        await _userRepository.DeleteUserAsync(user);
        return true;
    }

    public async Task<bool> DisableUserAsync(string userId)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user is null) throw new KeyNotFoundException("Usuario no encontrado");

        //Desabilitar cuenta
        user.LockoutEnabled = true;
        user.LockoutEnd = DateTimeOffset.UtcNow.AddYears(100);

        await _userManager.UpdateAsync(user);
        return true;
    }

    public async Task<bool> EnableUserAsync(string userId)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user is null) throw new KeyNotFoundException("Usuario no encontrado");
        //Habilitar cuenta
        user.LockoutEnabled = false;
        user.LockoutEnd = null;
        await _userManager.UpdateAsync(user);
        return true;
    }
}