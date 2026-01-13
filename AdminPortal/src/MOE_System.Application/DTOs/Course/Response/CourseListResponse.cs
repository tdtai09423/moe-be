using MOE_System.Domain.Enums;

namespace MOE_System.Application.DTOs.Course.Response;

public sealed record CourseListResponse
(
    string CourseCode,
    string CourseName,
    string ProviderName,
    string Mode,
    DateTime StartDate,
    DateTime EndDate,
    string PaymentType,
    decimal TotalFee,
    int EnrolledCount
);