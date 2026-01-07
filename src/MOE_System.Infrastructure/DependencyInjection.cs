using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using MOE_System.Infrastructure.Data;
using MOE_System.Application.Interfaces;
using MOE_System.Infrastructure.Repositories;

namespace MOE_System.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        // Register DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Register Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register Generic Repository
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        
        return services;
    }
}
