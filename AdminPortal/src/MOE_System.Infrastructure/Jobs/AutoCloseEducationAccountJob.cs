using MOE_System.Application.Interfaces.Services;
using Quartz;

namespace MOE_System.Infrastructure.Jobs;

[DisallowConcurrentExecution]
public sealed class AutoCloseEducationAccountJob : IJob
{
    private readonly IEducationAccountService _educationAccountService;

    public AutoCloseEducationAccountJob(IEducationAccountService educationAccountService)
    {
        _educationAccountService = educationAccountService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        await _educationAccountService.AutoCloseEducationAccountsAsync(cancellationToken: context.CancellationToken);
    }
}