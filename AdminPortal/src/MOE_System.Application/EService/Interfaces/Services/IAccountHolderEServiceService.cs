using MOE_System.Application.Admin.DTOs.AccountHolder;
using MOE_System.Application.EService.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace MOE_System.Application.EService.Interfaces.Services
{
    public interface IAccountHolderEServiceService
    {
        Task<AccountHolderInfoResponse> GetAccountHolderInformationAsync(string accountHolderId);
    }
}
