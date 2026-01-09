using MOE_System.Application.Common.Dashboard;

namespace MOE_System.Application.DTOs.Dashboard.Request;

public sealed record DashboardOverviewRequest
(
    DateRangeType DateRangeType = DateRangeType.AllTime,
    DateOnly? From = null,
    DateOnly? To = null
);