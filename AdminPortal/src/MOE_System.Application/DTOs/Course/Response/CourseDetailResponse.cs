namespace MOE_System.Application.DTOs.Course.Response;

public sealed record CourseDetailResponse
(
    string CourseCode,
    string CourseName,
    string ProviderName,
    string Mode,
    string Status,
    DateTime StartDate,
    DateTime EndDate,
    string PaymentType,
    string? BillingCycle,
    decimal TotalFee,
    IReadOnlyList<EnrolledStudent> EnrolledStudents
);

public sealed record EnrolledStudent
(
    string AccountHolderId,
    string StudentName,
    decimal TotalPaid,
    decimal TotalDue,
    DateTime EnrolledAt
);