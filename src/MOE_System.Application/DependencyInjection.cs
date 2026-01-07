using Microsoft.Extensions.DependencyInjection;

namespace MOE_System.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register Application services here
        // Example: services.AddScoped<IService, Service>();
        
        return services;
    }
}
