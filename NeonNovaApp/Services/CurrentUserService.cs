using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace NeonNovaApp.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly UserManager<Users> _userManager;
    private readonly IHttpContextAccessor _contextAccessor;

    public CurrentUserService(UserManager<Users> userManager, IHttpContextAccessor contextAccessor)
    {
        _userManager = userManager;
        _contextAccessor = contextAccessor;
    }

    public async Task<Users?> GetUser()
    {
        var emailClaim = _contextAccessor.HttpContext!.User.Claims.Where(c => c.Type == "email").FirstOrDefault();
        if (emailClaim is null) return null;
        var email = emailClaim.Value;
        return await _userManager.FindByEmailAsync(email);
    }
}