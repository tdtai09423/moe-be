using MOE_System.Application.DTOs;
using MOE_System.Application.Common;

namespace MOE_System.Application.Interfaces;

public interface IAccountHolderService
{
    Task<AccountHolderDto?> GetAccountHolderByIdAsync(string accountHolderId);
    Task<List<ActiveCourseDto>> GetActiveCoursesAsync(string accountHolderId);
    Task<PaginatedList<AccountHolderResponse>> GetAccountHoldersAsync(int pageNumber = 1, int pageSize = 20);
    Task<AccountHolderDetailResponse> GetAccountHolderDetailAsync(string accountHolderId);
    Task<AccountHolderResponse> AddAccountHolderAsync(CreateAccountHolderRequest request);
}
