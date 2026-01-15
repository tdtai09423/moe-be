namespace MOE_System.EService.Application.DTOs
{
    public class ActiveCoursesResponse
    {
        public string EnrollmentId { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public string ProviderName { get; set; } = string.Empty;
        public DateTime EnrollDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string PaymentType { get; set; } = string.Empty;
        public string? BillingCycle { get; set; }
        public DateTime? BillingDate { get; set; }
    }
}
