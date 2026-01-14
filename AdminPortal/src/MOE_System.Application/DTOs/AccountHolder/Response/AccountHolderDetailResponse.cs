namespace MOE_System.Application.DTOs.AccountHolder.Response
{
    public class AccountHolderDetailResponse
    {
        public string FullName { get; set; } = string.Empty;
        public string NRIC { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public int CourseCount { get; set; }
        public decimal OutstandingFees { get; set; }
        public bool? IsActive { get; set; } = null;
        public required StudentInformation StudentInformation { get; set; }
        public List<EnrolledCourseInfo> EnrolledCourses { get; set; } = new();
        public List<OutstandingFeeInfo> OutstandingFeesDetails { get; set; } = new();
        public List<TopUpHistoryInfo> TopUpHistory { get; set; } = new();
        public List<PaymentHistoryInfo> PaymentHistory { get; set; } = new();

    }

    public class StudentInformation
    {
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; } = string.Empty; 
        public string ContactNumber { get; set; } = string.Empty;
        public string EducationLevel { get; set; } = string.Empty;
        public string ResidentialStatus { get; set; } = string.Empty;
        public string SchoolingStatus { get; set; } = string.Empty;
        public string RegisteredAddress { get; set; } = string.Empty;
        public string MailingAddress { get; set; } = string.Empty;
        public string CreatedAt { get; set; } =string.Empty;
    }

    public class EnrolledCourseInfo
    {
        public string CourseName { get; set; } = string.Empty;
        public string BillingCycle { get; set; } = string.Empty;
        public decimal TotalFree { get; set; }
        public string EnrollmentDate { get; set; } = string.Empty;
        public decimal CollectedFee { get; set; }
        public string NextPaymentDue { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
    }

    public class OutstandingFeeInfo
    {
        public string CourseName { get; set; } = string.Empty;
        public decimal OutstandingAmount { get; set; }
        public string BillingDate { get; set; } = string.Empty;
        public string DueDate { get; set; } = string.Empty;
    }
    
    public class TopUpHistoryInfo
    {
        public DateTime TopUpTime { get; set; }
        public decimal Amount { get; set; }
        public string Reference { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class PaymentHistoryInfo
    {
        public string CourseName { get; set; } = string.Empty;
        public decimal AmountPaid { get; set; }
        public string PaymentDate { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
    }
}
