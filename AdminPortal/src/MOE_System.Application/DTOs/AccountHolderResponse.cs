namespace MOE_System.Application.DTOs
{
    public class AccountHolderResponse
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string NRIC { get; set; } = string.Empty;
        public int Age { get; set; }
        public decimal Balance { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public string EducationLevel { get; set; } = string.Empty;
        public int CourseCount { get; set; }
        public decimal OutstandingFees { get; set; }
    }
}
