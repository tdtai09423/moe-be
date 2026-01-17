namespace MOE_System.Application.DTOs.Dashboard.Response;

public sealed record ScheduledTopUpResponse
(
    string TopupRuleId,
    string Name,
    decimal TopUpAmount,
    DateTime ScheduledTime,
    string Status,
    int? NumberOfAccountsAffected
);