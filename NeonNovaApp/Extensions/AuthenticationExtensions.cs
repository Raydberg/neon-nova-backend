using System.Text;
using Domain.Entities;
using Intrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace NeonNovaApp.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddAuthenticationConfiguration(this IServiceCollection services)
    {
        var KeyJwt = Environment.GetEnvironmentVariable("key_jwt");

        services.AddIdentityCore<Users>(opt =>
            {
                opt.SignIn.RequireConfirmedEmail = true;
                opt.Password.RequiredLength = 6;
                opt.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<UserManager<Users>>();
        services.AddScoped<SignInManager<Users>>();
        services.AddHttpContextAccessor();

        services.AddAuthentication().AddJwtBearer(opt =>
        {
            opt.MapInboundClaims = false;
            opt.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KeyJwt!)),
                ClockSkew = TimeSpan.Zero
            };
        });

        services.AddAuthorization(opt =>
        {
            opt.AddPolicy("isAdmin", politica => politica.RequireClaim("isAdmin"));
            opt.AddPolicy("isUser", politica => politica.RequireClaim("isUser"));
            opt.AddPolicy("isInvited", politica => politica.RequireClaim("isInvited"));
        });

        return services;
    }
}