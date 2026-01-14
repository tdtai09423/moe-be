using MOE_System.Application.DTOs.Dashboard.Response;
using MOE_System.Application.Interfaces.Services;
using MOE_System.Domain.Entities;
using MOE_System.Application.Common.Interfaces;
using MOE_System.Application.Common.Dashboard;
using Microsoft.EntityFrameworkCore;

namespace MOE_System.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IUnitOfWork _unitOfWork;

    public DashboardService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task<IReadOnlyList<ScheduledTopUpResponse>> GetTopUpTypesAsync(ScheduledTopUpTypes type, CancellationToken cancellationToken)
    {
        return type switch 
        { 
            ScheduledTopUpTypes.Batch => QueryScheduledTopUpAsync("BATCH", cancellationToken),
            ScheduledTopUpTypes.Individual => QueryScheduledTopUpAsync("INDIVIDUAL", cancellationToken),
            _ => throw new ArgumentOutOfRangeException(nameof(type), $"The scheduled top-up type '{type}' is not recognized.")
        };
    }

    private async Task<IReadOnlyList<ScheduledTopUpResponse>> QueryScheduledTopUpAsync(string targetType, CancellationToken cancellationToken)
    {
        var batchRuleRepository = _unitOfWork.GetRepository<BatchRuleExecution>();
        var now = DateTime.UtcNow;

        var results = await batchRuleRepository.ToListAsync(
            predicate: br =>
                br.TopupRule != null &&
                br.BatchExecution != null &&
                br.TopupRule.RuleTargetType == targetType &&
                br.BatchExecution.ScheduledTime >= now &&
                br.BatchExecution.Status == "SCHEDULED",
            include: query => query
                .Include(x => x.TopupRule)
                    .ThenInclude(r => r!.TargetEducationAccount)
                        .ThenInclude(a => a!.AccountHolder)
                .Include(x => x.BatchExecution),
            orderBy: query => query.OrderBy(x => x.BatchExecution!.ScheduledTime),
            take: 5,
            cancellationToken: cancellationToken
        );

        return results.Select(x =>
        {
            string name;

            if (targetType == "BATCH")
            {
                name = x.TopupRule!.RuleName;
            }
            else
            {
                name = x.TopupRule!.TargetEducationAccount!.AccountHolder!.FullName;
            }

            return new ScheduledTopUpResponse(
                name,
                x.TopupRule.TopupAmount,
                x.BatchExecution!.ScheduledTime,
                x.BatchExecution!.Status
            );
        }).ToList();
    }

    public async Task<IReadOnlyList<RecentActivityResponse>> GetRecentActivitiesAsync(CancellationToken cancellationToken)
    {
        var educationAccountRepository = _unitOfWork.GetRepository<EducationAccount>();

        var results = await educationAccountRepository.ToListAsync(
            include: query => query
                .Include(e => e.AccountHolder),
            orderBy: query => query.OrderByDescending(e => e.CreatedAt),
            take: 10,
            cancellationToken: cancellationToken
        );

        return results.Select(e => new RecentActivityResponse(
            e.AccountHolder!.FullName,
            e.AccountHolder.Email,
            e.CreatedAt
        )).ToList();
    }
}
