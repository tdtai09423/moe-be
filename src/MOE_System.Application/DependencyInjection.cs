using Microsoft.Extensions.DependencyInjection;
using MOE_System.Application.Interfaces;
using MOE_System.Application.Services;

namespace MOE_System.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register Application services here
        services.AddScoped<IAccountHolderService, AccountHolderService>();
        services.AddScoped<IEducationAccountService, EducationAccountService>();
        
        return services;
    }
}
