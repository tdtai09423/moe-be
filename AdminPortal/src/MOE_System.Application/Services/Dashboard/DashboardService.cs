using MOE_System.Application.DTOs.Dashboard.Response;
using MOE_System.Application.Interfaces.Services;
using MOE_System.Domain.Entities;
using MOE_System.Application.Common.Interfaces;
using MOE_System.Application.Common.Dashboard;

namespace MOE_System.Application.Services.Dashboard;

public class DashboardService : IDashboardService
{
    private readonly IUnitOfWork _unitOfWork;

    public DashboardService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<DashboardOverviewResponse> GetDashboardOverviewAsync(
        DateRangeType dateRangeType,
        DateOnly? from,
        DateOnly? to,
        CancellationToken cancellationToken = default
    )
    {
        var nowUtc = DateTime.UtcNow;

        var dateRange = DateRangeResolver.Resolve(
            dateRangeType,
            nowUtc,
            from,
            to
        );

        var hocRepo = _unitOfWork.GetRepository<HistoryOfChange>();
        var transactionRepo = _unitOfWork.GetRepository<Transaction>();
        var invoiceRepo = _unitOfWork.GetRepository<Invoice>();
        
        var totalDisbursed = await hocRepo.SumAsync(
            predicate: dateRange is null
                ? h => h.Type == "Top-Up"
                : h => h.Type == "Top-Up" &&
                       h.CreatedAt >= dateRange.StartDate &&
                       h.CreatedAt < dateRange.EndDate,
            selector: h => h.Amount,
            cancellationToken: cancellationToken
        );

        var totalCollected = await transactionRepo.SumAsync(
            predicate: dateRange is null
                ? t => t.Status == "Completed"
                : t => t.Status == "Completed" &&
                       t.TransactionAt >= dateRange.StartDate &&
                       t.TransactionAt < dateRange.EndDate,
            selector: t => t.Amount,
            cancellationToken: cancellationToken
        );

        var outstandingPayments = await invoiceRepo.SumAsync(
            predicate: i => i.Status == "Unpaid",
            selector: i => i.Amount,
            cancellationToken: cancellationToken
        );
        
        return new DashboardOverviewResponse(
            totalDisbursed, 
            totalCollected, 
            outstandingPayments,
            nowUtc
        );
    }
}
