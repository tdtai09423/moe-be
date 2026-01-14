using MOE_System.EService.Application.Common;
using MOE_System.EService.Application.DTOs;

namespace MOE_System.EService.Application.Interfaces.Services
{
    public interface IEnrollmentService
    {
        Task<PaginatedList<ActiveCoursesResponse>> GetActiveCoursesAsync(string accountHolderId, int pageIndex, int pageSize);
    }
}
