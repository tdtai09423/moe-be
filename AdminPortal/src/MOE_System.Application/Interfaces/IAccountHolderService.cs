using MOE_System.Application.DTOs;
using MOE_System.Application.Common;
using MOE_System.Application.DTOs.AccountHolder;
using MOE_System.Application.DTOs.AccountHolder.Request;
using MOE_System.Application.DTOs.AccountHolder.Response;

namespace MOE_System.Application.Interfaces;

public interface IAccountHolderService
{
    Task<PaginatedList<AccountHolderResponse>> GetAccountHoldersAsync(int pageNumber = 1, int pageSize = 20);
    Task<AccountHolderDetailResponse> GetAccountHolderDetailAsync(string accountHolderId);
    Task<AccountHolderResponse> AddAccountHolderAsync(CreateAccountHolderRequest request);
}
