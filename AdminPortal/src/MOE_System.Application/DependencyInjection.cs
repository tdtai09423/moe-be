using Microsoft.Extensions.DependencyInjection;
using MOE_System.Application.Interfaces;
using MOE_System.Application.Services;
using MOE_System.Application.Interfaces.Services;
using MOE_System.Application.Services.Dashboard;
using FluentValidation;

namespace MOE_System.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        
        // Register Application services
        services.AddScoped<IAccountHolderService, AccountHolderService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<ICourseService, CourseService>();
        
        return services;
    }
}
