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
            ScheduledTopUpTypes.Batch => GetBatchScheduledTopUpAsync(cancellationToken),
            _ => throw new NotImplementedException($"The scheduled top-up type '{type}' is not implemented.")
        };
    }

    private async Task<IReadOnlyList<ScheduledTopUpResponse>> GetBatchScheduledTopUpAsync(CancellationToken cancellationToken)
    {
        var batchRuleRepository = _unitOfWork.GetRepository<BatchRuleExecution>();

        var now = DateTime.UtcNow;

        var results = await batchRuleRepository.ToListAsync(
            predicate: x => 
                x.BatchExecution != null &&
                x.BatchExecution.Status == "Scheduled" && 
                x.BatchExecution.ScheduledTime >= now,
            include: x => x
                .Include(y => y.BatchExecution)
                .Include(z => z.TopupRule),
            orderBy: x => x.OrderBy(y => y.BatchExecution!.ScheduledTime),
            take: 5,
            cancellationToken: cancellationToken
        );

        return results.Select(x => new ScheduledTopUpResponse(
            x.TopupRule!.RuleName,
            x.TopupRule!.TopupAmount,
            x.BatchExecution!.ScheduledTime,
            x.BatchExecution!.Status
        )).ToList();
    }

    public Task<IReadOnlyList<RecentActivityResponse>> GetRecentActivitiesAsync(RecentActivityTypes type, CancellationToken cancellationToken)
    {
        return type switch 
        { 
            RecentActivityTypes.Accounts => GetLatestAccountAsync(cancellationToken),
            RecentActivityTypes.Enrollments => GetLatestEnrollmentsAsync(cancellationToken),
            _ => throw new NotImplementedException($"The recent activity type '{type}' is not implemented.")
        };
    }

    private async Task<IReadOnlyList<RecentActivityResponse>> GetLatestAccountAsync(CancellationToken cancellationToken)
    {
        var educationAccountRepository = _unitOfWork.GetRepository<EducationAccount>();

        var results = await educationAccountRepository.ToListAsync(
            include: x => x.Include(x => x.AccountHolder),
            orderBy: x => x.OrderByDescending(y => y.CreatedAt),
            take: 10,
            cancellationToken: cancellationToken
        );

        return results.Select(x => new RecentActivityResponse(
            null,
            null,
            x.AccountHolder?.FullName,
            x.AccountHolder?.Email,
            x.CreatedAt
        )).ToList();
    }

    private async Task<IReadOnlyList<RecentActivityResponse>> GetLatestEnrollmentsAsync(CancellationToken cancellationToken)
    {
        var enrollmentRepository = _unitOfWork.GetRepository<Enrollment>();

        var results = await enrollmentRepository.ToListAsync(
            include: x => x
                .Include(x => x.Course)
                .Include(x => x.EducationAccount!)
                    .ThenInclude(y => y.AccountHolder),
            orderBy: x => x.OrderByDescending(y => y.EnrollDate),
            take: 10,
            cancellationToken: cancellationToken
        );

        return results.Select(x => new RecentActivityResponse(
            x.EducationAccount?.AccountHolder?.FullName,
            x.Course?.CourseName,
            null,
            null,
            x.EnrollDate
        )).ToList();
    }
}
