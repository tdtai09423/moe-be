using System;
using System.Collections.Generic;
using System.Text;

namespace MOE_System.EService.Application.DTOs.Dashboard
{
    public class DashboardResponse
    {
        public string FullName { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public int ActiveCoursesCount { get; set; }
        public decimal OutstandingFees { get; set; }
        public int OutstadingCount { get; set; }
        public List<EnrollCourse> EnrollCourses { get; set; } = new List<EnrollCourse>();
    }

    public class  EnrollCourse
    {
        public string CourseName { get; set; } = string.Empty;
        public string ProviderName { get; set; } = string.Empty;
        public string PaymentType { get; set; } = string.Empty;
        public string BillingCycle { get; set; } = string.Empty;
        public string EnrollDate { get; set; } = string.Empty;
        public string BillingDate { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
    }
}
