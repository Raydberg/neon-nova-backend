using System.Reflection;
using Application.Interfaces;
using Application.Services;
using Domain.Interfaces;
using Intrastructure.Repositories;
using NeonNovaApp.Services;

namespace NeonNovaApp.Extensions;

public static class ApplicationServicesExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Registrar Servicios
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddTransient<ICurrentUserService, CurrentUserService>();
        services.AddTransient<ICategoryService, CategoryService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IProductImageService, ProductImageService>();
        services.AddScoped<ICloudinaryService, CloudinaryService>();
        services.AddScoped<IUserService, UserService>();

        // Registrar Repositorios
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IAuthRepository, AuthRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductImageRepository, ProductImageRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }

    // Registrar Mappers
    public static IServiceCollection AddMappingConfiguration(this IServiceCollection services)
    {
        services.AddAutoMapper(
            Assembly.GetExecutingAssembly(),
            typeof(Application.Mappings.MappingProduct).Assembly,
            typeof(Application.Mappings.MappingUser).Assembly,
            typeof(Application.Mappings.MappingCategory).Assembly
        );

        return services;
    }
}