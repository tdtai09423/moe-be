using System;
using System.Collections.Generic;
using System.Text;

namespace MOE_System.Application.EService.DTOs
{
    public class ActiveCourseForAccountResponse
    {
        public string EnrollmentId { get; set; }
        public string CourseName { get; set; }
        public decimal FeeAmount { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; }
    }
}
