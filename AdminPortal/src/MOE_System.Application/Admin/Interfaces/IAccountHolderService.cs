using MOE_System.Application.Admin.DTOs.AccountHolder;
using MOE_System.Application.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace MOE_System.Application.Admin.Interfaces
{
    public interface IAccountHolderService
    {
        Task<PaginatedList<AccountHolderResponse>> GetAccountHoldersAsync(int pageNumber = 1, int pageSize = 20);
        Task<AccountHolderDetailResponse> GetAccountHolderDetailAsync(string accountHolderId);
        Task<AccountHolderResponse> AddAccountHolderAsync(CreateAccountHolderRequest request);
    }
}
