using MOE_System.Application.Common.Dashboard;
using MOE_System.Application.DTOs.Dashboard.Response;

namespace MOE_System.Application.Interfaces.Services;

public interface IDashboardService
{
    Task<IReadOnlyList<ScheduledTopUpResponse>> GetTopUpTypesAsync(ScheduledTopUpTypes type, CancellationToken cancellationToken);
    Task<IReadOnlyList<RecentActivityResponse>> GetRecentActivitiesAsync(RecentActivityTypes type, CancellationToken cancellationToken);
}