using System;
using System.Collections.Generic;
using System.Text;

namespace MOE_System.EService.Application.DTOs.AccountHolder
{
    public class YourCourseResponse
    {
        public decimal OutstandingFees { get; set; } 
        public decimal Balance { get; set; }
        public List<EnrolledCourse> EnrolledCourses { get; set; } = new List<EnrolledCourse>();
        public List<PendingFees> PendingFees { get; set; } = new List<PendingFees>();
        public List<PaymentHistory> PaymentHistory { get; set; } = new List<PaymentHistory>();
    }
    public class EnrolledCourse
    {
        public string CourseName { get; set; } = string.Empty;
        public string ProviderName { get; set; } = string.Empty;
        public decimal CourseFee { get; set; }
        public string BillingCycle { get; set; } = string.Empty;
        public string EnrolledDate { get; set; } = string.Empty;
        public string BillingDate { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
    }

    public class PendingFees
    {
        public string CourseName { get; set; } = string.Empty;
        public string ProviderName { get; set; } = string.Empty;
        public decimal AmountDue { get; set; }
        public string BillingCycle { get; set; } = string.Empty;
        public string BillingDate { get; set; } = string.Empty;
        public string DueDate { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
    }

    public class PaymentHistory
    {
        public string CourseName { get; set; } = string.Empty;
        public string ProviderName { get; set; } = string.Empty;
        public decimal AmountPaid { get; set; }
        public string BillingCycle { get; set; } = string.Empty;
        public string PaymentDate { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
    }
}
