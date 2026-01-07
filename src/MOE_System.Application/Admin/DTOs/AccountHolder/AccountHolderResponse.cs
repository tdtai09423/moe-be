using System;
using System.Collections.Generic;
using System.Text;

namespace MOE_System.Application.Admin.DTOs.AccountHolder
{
    public class AccountHolderResponse
    {
        public string FullName { get; set; } = string.Empty;
        public string NRIC { get; set; } = string.Empty;
        public int Age { get; set; }
        public decimal Balance { get; set; }
        public string SchoolingStatus { get; set; } = string.Empty;
        public string EducationLevel { get; set; } = string.Empty;
        public int CourseCount { get; set; }
        public decimal OutstandingFees { get; set; }
    }
}
