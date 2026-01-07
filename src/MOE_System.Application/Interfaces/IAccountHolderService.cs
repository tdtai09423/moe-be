using MOE_System.Application.DTOs.AccountHolder;
using System;
using System.Collections.Generic;
using System.Text;

namespace MOE_System.Application.Interfaces
{
    public interface IAccountHolderService
    {
        Task<List<AccountHolderResponse>> GetAccountHoldersAsync();
        Task<AccountHolderDetailResponse> GetAccountHolderDetailAsync(int accountHolderId);
    }
}
