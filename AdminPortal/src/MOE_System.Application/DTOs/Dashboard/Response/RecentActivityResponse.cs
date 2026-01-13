namespace MOE_System.Application.DTOs.Dashboard.Response;

public sealed record RecentActivityResponse(
    string? StudentName,
    string? CourseName,
    string? Name,
    string? Email,
    DateTime ActivityDate
);