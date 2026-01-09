using MOE_System.Application.Common.Dashboard;
using MOE_System.Application.DTOs.Dashboard.Response;

namespace MOE_System.Application.Interfaces.Services;

public interface IDashboardService
{
    Task<DashboardOverviewResponse> GetDashboardOverviewAsync(
        DateRangeType dateRangeType,
        DateOnly? from,
        DateOnly? to,
        CancellationToken cancellationToken = default
    );
}