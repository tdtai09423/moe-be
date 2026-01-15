using System;
using System.Collections.Generic;
using System.Text;

namespace MOE_System.Application.DTOs.Course.Request
{
    public class BulkRemoveEnrolledAccountRequest
    {
        public string CourseId { get; set; } = string.Empty;
        public List<string> EducationAccountIds { get; set; } = new List<string>();
    }
}
