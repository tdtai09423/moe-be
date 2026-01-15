using MOE_System.Application.Common.Course;

namespace MOE_System.Application.DTOs.Course.Request;

public sealed record GetCourseRequest
(
    int PageNumber = 1,
    int PageSize = 10,
    string? SearchTerm = null,
    List<string>? Provider = null,
    List<string>? ModeOfTraining = null,
    List<string>? Status = null,
    List<string>? PaymentType = null,
    List<string>? BillingCycle = null,
    DateOnly? StartDate = null,
    DateOnly? EndDate = null,
    decimal? TotalFeeMin = null,
    decimal? TotalFeeMax = null,
    CourseSortField SortBy = CourseSortField.CreatedAt,
    SortDirection SortDirection = SortDirection.Desc
);