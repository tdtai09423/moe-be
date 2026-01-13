using MOE_System.Application.DTOs.Dashboard.Response;
using MOE_System.Application.Interfaces.Services;
using MOE_System.Domain.Entities;
using MOE_System.Application.Common.Interfaces;
using MOE_System.Application.Common.Dashboard;
using Microsoft.EntityFrameworkCore;

namespace MOE_System.Application.Services.Dashboard;

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
            take: 5
        );

        return results.Select(x => new ScheduledTopUpResponse(
            x.TopupRule!.RuleName,
            x.TopupRule!.TopupAmount,
            x.BatchExecution!.ScheduledTime,
            x.BatchExecution!.Status
        )).ToList();
    }
}
