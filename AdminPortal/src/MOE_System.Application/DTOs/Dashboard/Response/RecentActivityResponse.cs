namespace MOE_System.Application.DTOs.Dashboard.Response;

public sealed record RecentActivityResponse(
    string Name,
    string Email,
    DateTime CreatedAt
);