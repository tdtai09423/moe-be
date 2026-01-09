using MOE_System.Domain.Entities;

namespace MOE_System.EService.Application.Interfaces
{
    public interface IEnrollmentRepository
    {
        Task<List<Enrollment>> GetActiveCoursesForAccountAsync(string accountHolderId);
        Task<List<Enrollment>> GetOutstandingFeeForAccountAsync(string accountHolderId);
    }
}
