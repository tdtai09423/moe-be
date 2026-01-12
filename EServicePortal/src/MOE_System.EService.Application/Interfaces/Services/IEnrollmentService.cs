using MOE_System.EService.Application.DTOs;

namespace MOE_System.EService.Application.Interfaces.Services
{
    public interface IEnrollmentService
    {
        Task<IEnumerable<ActiveCoursesResponse>> GetActiveCoursesAsync(string accountHolderId);
    }
}
