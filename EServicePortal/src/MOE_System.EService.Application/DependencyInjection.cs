using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using MOE_System.EService.Application.Interfaces.Services;
using MOE_System.EService.Application.Services;

namespace MOE_System.EService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        
        // Register Application services
        services.AddScoped<IAccountHolderEServiceService, AccountHolderEServiceService>();
        services.AddScoped<IEducationAccountService, EducationAccountService>();
        services.AddScoped<IEnrollmentService, EnrollmentService>();

        return services;
    }
}
