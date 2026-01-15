using System;
using System.Collections.Generic;
using System.Text;

namespace MOE_System.Application.DTOs.Course.Response
{
    public class NonEnrolledAccountResponse
    {
        public List<NonEnrolledAccountDetailResponse> NonEnrolledAccounts { get; set; } = new List<NonEnrolledAccountDetailResponse>();
    }

    public class NonEnrolledAccountDetailResponse
    {
        public string EducationAccountId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string NRIC { get; set; } = string.Empty;
    }
}
