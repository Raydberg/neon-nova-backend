namespace NeonNovaApp.Extensions;

public static class HttpClientExtensions
{
    public static IServiceCollection AddHttpServices(this IServiceCollection services)
    {
        services.AddHttpClient();
        
        services.AddHttpClient("GoogleImageProxy", client =>
        {
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.0.0 Safari/537.36");
            client.Timeout = TimeSpan.FromSeconds(15);
        }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            AllowAutoRedirect = true,
            MaxAutomaticRedirections = 5,
            AutomaticDecompression = System.Net.DecompressionMethods.All,
            UseCookies = false
        });
        
        return services;
    }
}