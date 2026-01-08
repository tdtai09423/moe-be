using Microsoft.Extensions.DependencyInjection;
using MOE_System.Application.Admin.Interfaces;
using MOE_System.Application.Admin.Services;
using MOE_System.Application.EService.Interfaces.Services;
using MOE_System.Application.EService.Services;

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
        services.AddScoped<IAccountHolderEServiceService, AccountHolderEServiceService>();
        services.AddScoped<IEducationAccountService, EducationAccountService>();
        services.AddScoped<IEnrollmentService, EnrollmentService>();
        return services;
    }
}
