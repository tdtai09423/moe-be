using Microsoft.Extensions.DependencyInjection;
using MOE_System.Application.Interfaces;
using MOE_System.Application.Services.Admin;

namespace MOE_System.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register Application services here
        // Example: services.AddScoped<IService, Service>();
        services.AddScoped<IAccountHolderService, AccountHolderService>();
        return services;
    }
}
