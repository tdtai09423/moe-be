using MOE_System.Application.Common.Course;

namespace MOE_System.Application.DTOs.Course.Request;

public sealed record GetCourseRequest
(
    int PageNumber = 1,
    int PageSize = 10,
    string? SearchTerm = null,
    string? Provider = null,
    string? Mode = null,
    string? PaymentType = null,
    string? BillingCycle = null,
    DateTime? StartDateFrom = null,
    DateTime? StartDateTo = null,
    DateTime? EndDateFrom = null,
    DateTime? EndDateTo = null,
    decimal? TotalFeeMin = null,
    decimal? TotalFeeMax = null,
    CourseSortField SortBy = CourseSortField.CreatedAt,
    SortDirection SortDirection = SortDirection.Desc
);