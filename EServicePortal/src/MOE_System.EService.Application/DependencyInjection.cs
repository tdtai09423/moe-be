using Microsoft.Extensions.DependencyInjection;
using FluentValidation;

namespace MOE_System.EService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        
        // Register Application services here
        // Example: services.AddScoped<IService, Service>();
        
        #region EService Services
        // services.AddScoped<IEService, EService>();
        #endregion

        return services;
    }
}
