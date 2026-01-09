namespace MOE_System.Application.DTOs.Dashboard.Response;

public sealed record DashboardOverviewResponse
(
    decimal TotalDisbursed,
    decimal TotalCollected,
    decimal OutstandingPayments,
    DateTime OutstandingAsOfUtc
);