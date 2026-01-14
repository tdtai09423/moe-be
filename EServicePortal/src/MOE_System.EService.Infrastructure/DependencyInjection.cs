using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using MOE_System.EService.Infrastructure.Data;
using MOE_System.EService.Infrastructure.Repositories;
using MOE_System.EService.Application.Common.Interfaces;
using MOE_System.EService.Infrastructure.Services;
using MOE_System.EService.Application.Interfaces;

namespace MOE_System.EService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Get connection string from configuration
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        
        // Register DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Register Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register Generic Repository
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        // Register Custom Repositories

        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<MOE_System.EService.Application.Interfaces.Services.IJwtService, JwtService>();

        return services;
    }
}
