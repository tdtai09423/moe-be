using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using MOE_System.Infrastructure.Data;
using MOE_System.Infrastructure.Repositories;
using MOE_System.Application.Common.Interfaces;
using MOE_System.Application.Interfaces;
using MOE_System.Infrastructure.Services;
using MOE_System.Application.Common;
using MOE_System.Infrastructure.Common;
using Quartz;
using MOE_System.Infrastructure.Jobs;

namespace MOE_System.Infrastructure;

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

        services.Configure<AccountClosureOptions>(options =>
            configuration.GetSection("ClosureAccountOptions").Bind(options)
        );

        services.AddQuartz(q =>
        {
            var jobKey = new JobKey("AutoCloseEducationAccountJob");

            q.AddJob<AutoCloseEducationAccountJob>(opts => opts.WithIdentity(jobKey));

            q.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity("AutoCloseEducationAccountTrigger")
                .WithSchedule(
                    CronScheduleBuilder.DailyAtHourAndMinute(0, 0)
                ));
        });

        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

        // Register Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register Generic Repository
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<IClock, SystemClock>();

        return services;
    }
}
