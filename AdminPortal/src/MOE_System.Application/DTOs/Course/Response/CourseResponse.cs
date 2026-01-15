namespace MOE_System.Application.DTOs.Course.Response
{
    public class CourseResponse
    {
        public string Id { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public string ProviderId { get; set; } = string.Empty;
        public string ProviderName { get; set; } = string.Empty;
        public string ModeOfTraining { get; set; } = string.Empty;
        public DateTime CourseStartDate { get; set; }
        public DateTime CourseEndDate { get; set; }
        public string PaymentOption { get; set; } = string.Empty;
        public decimal TotalFee { get; set; }
        public string? BillingCycle { get; set; }
        public decimal? FeePerCycle { get; set; }
        public string? TermName { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
