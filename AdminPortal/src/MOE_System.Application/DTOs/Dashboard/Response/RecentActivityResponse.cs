namespace MOE_System.Application.DTOs.Dashboard.Response;

public sealed record RecentActivityResponse(
    string AccountId,
    string Name,
    string Email,
    DateTime CreatedAt
);