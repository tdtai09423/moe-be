using MOE_System.EService.Application.DTOs.AccountHolder;

namespace MOE_System.EService.Application.Interfaces.Services
{
    public interface IAccountHolderService
    {
        Task<AccountHolderResponse> GetAccountHolderAsync(string accountHolderId);
        Task<AccountHolderProfileResponse> GetMyProfileAsync(string accountHolderId);
        Task<YourCourseResponse> GetYourCoursesAsync(string accountHolderId);
        Task<UpdateProfileResponse> UpdateProfileAsync(string accountHolderId, UpdateProfileRequest request);
    }
}
