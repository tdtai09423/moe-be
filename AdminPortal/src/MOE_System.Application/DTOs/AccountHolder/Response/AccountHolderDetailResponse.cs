namespace MOE_System.Application.DTOs.AccountHolder.Response
{
    public class AccountHolderDetailResponse
    {
        public decimal Balance { get; set; }
        public int CourseCount { get; set; }
        public decimal OutstandingFees { get; set; }
        public decimal TotalFeesPaid { get; set; }
        public required StudentInformation StudentInformation { get; set; }
        public List<EnrolledCourseInfo> EnrolledCourses { get; set; } = new();
        public List<OutstandingFeeInfo> OutstandingFeesDetails { get; set; } = new();
        public List<PaymentHistoryInfo> PaymentHistory { get; set; } = new();

    }

    public class StudentInformation
    {
        public string FullName { get; set; } = string.Empty;
        public string NRIC { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; } = string.Empty; 
        public string ContactNumber { get; set; } = string.Empty;
        public string SchoolingStatus { get; set; } = string.Empty;
        public string EducationLevel { get; set; } = string.Empty;
        public string RegisteredAddress { get; set; } = string.Empty;
        public string MailingAddress { get; set; } = string.Empty;
        public bool? IsActive { get; set; } = null;
        public DateTime CreatedAt { get; set; }
    }

    public class EnrolledCourseInfo
    {
        public string CourseName { get; set; } = string.Empty;
        public string BillingCycle { get; set; } = string.Empty;
        public decimal TotalFree { get; set; }
        public decimal CollectedFee { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public string NextPaymentDue { get; set; } = string.Empty;
        public DateTime EnrollmentDate { get; set; }
    }

    public class OutstandingFeeInfo
    {
        public string CourseName { get; set; } = string.Empty;
        public decimal OutstandingAmount { get; set; }
        public DateTime DueDate { get; set; }
        public string PaymentStatus { get; set; } = string.Empty; 
    }

    public class PaymentHistoryInfo
    {
        public string CourseName { get; set; } = string.Empty;
        public decimal AmountPaid { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
