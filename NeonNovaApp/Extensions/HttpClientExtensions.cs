namespace NeonNovaApp.Extensions;

public static class HttpClientExtensions
{
    public static IServiceCollection AddHttpServices(this IServiceCollection services)
    {
        services.AddHttpClient();
        
        return services;
    }
}