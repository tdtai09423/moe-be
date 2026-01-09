using MOE_System.Application.EService.DTOs;
using MOE_System.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MOE_System.Application.EService.Interfaces.Services
{
    public interface IEnrollmentService
    {
        Task<IEnumerable<ActiveCourseForAccountResponse>> GetActiveCoursesForAccountAsync(string accountHolderId);
        Task<EducationAccountOutstandingFeeResponse> GetOutstandingFeeAsync(string educationAccountId);
    }
}
