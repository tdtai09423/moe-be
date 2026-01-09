using MOE_System.EService.Application.DTOs;

namespace MOE_System.EService.Application.Interfaces.Services
{
    public interface IEnrollmentService
    {
        Task<List<ActiveCourseForAccountResponse>> GetActiveCoursesForAccountAsync(string accountHolderId);
        Task<EducationAccountOutstandingFeeResponse> GetOutstandingFeeForAccountAsync(string accountHolderId);
    }
}
