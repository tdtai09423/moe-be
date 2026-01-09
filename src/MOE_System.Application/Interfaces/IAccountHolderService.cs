using MOE_System.Application.DTOs;

namespace MOE_System.Application.Interfaces;

public interface IAccountHolderService
{
    Task<AccountHolderDto?> GetAccountHolderByIdAsync(string accountHolderId);
    Task<List<ActiveCourseDto>> GetActiveCoursesAsync(string accountHolderId);
}
