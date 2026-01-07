using Microsoft.Extensions.DependencyInjection;
using MOE_System.Application.Admin.Interfaces;
using MOE_System.Application.Admin.Services;

namespace MOE_System.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register Application services here
        // Example: services.AddScoped<IService, Service>();
        #region Admin Services
            services.AddScoped<IAccountHolderService, AccountHolderService>();
        #endregion

        #region EServices
        // services.AddScoped<IEService, EService>();
        #endregion
        return services;
    }
}
