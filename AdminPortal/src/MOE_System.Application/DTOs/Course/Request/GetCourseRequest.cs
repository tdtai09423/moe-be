namespace MOE_System.Application.DTOs.Course.Request;

public sealed record GetCourseRequest
(
    int PageNumber = 1,
    int PageSize = 10,
    string? SearchTerm = null
);