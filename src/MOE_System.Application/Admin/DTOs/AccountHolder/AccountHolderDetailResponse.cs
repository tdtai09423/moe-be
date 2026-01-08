using System;
using System.Collections.Generic;
using System.Text;

namespace MOE_System.Application.Admin.DTOs.AccountHolder
{
    public class AccountHolderDetailResponse
    {
        public decimal Balance { get; set; }
        public int ActiveCourseCount { get; set; }
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
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class EnrolledCourseInfo
    {
        public string CourseName { get; set; } = string.Empty;
        public DateTime EnrollmentDate { get; set; }
        public decimal CourseFee { get; set; }
        public string Status { get; set; } = string.Empty;
    }
    
    public class OutstandingFeeInfo
    {
        public string CourseName { get; set; } = string.Empty;
        public decimal OutstandingAmount { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; } = string.Empty; 
    }

    public class PaymentHistoryInfo
    {
        public string CourseName { get; set; } = string.Empty;
        public decimal AmountPaid { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}


