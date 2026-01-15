using System;
using System.Collections.Generic;
using System.Text;

namespace MOE_System.Application.DTOs.Course.Request
{
    public class BulkEnrollAccountAsync
    {
        public string CourseId { get; set; } = string.Empty;
        public List<string> AccountIds { get; set; } = new List<string>();
    }
}
