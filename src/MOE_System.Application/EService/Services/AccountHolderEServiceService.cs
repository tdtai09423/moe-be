using MOE_System.Application.Admin.DTOs.AccountHolder;
using MOE_System.Application.EService.DTOs;
using MOE_System.Application.EService.Interfaces.Repositories;
using MOE_System.Application.EService.Interfaces.Services;
using MOE_System.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace MOE_System.Application.EService.Services
{
    public class AccountHolderEServiceService : IAccountHolderEServiceService
    {
        private readonly IAccountHolderRepository _accountHolderRepo;

        public AccountHolderEServiceService(IAccountHolderRepository accountHolderRepo)
        {
            _accountHolderRepo = accountHolderRepo;
        }

        public async Task<AccountHolderInfoResponse> GetAccountHolderInformationAsync(string accountHolderId)
        {
            var accountHolder = await _accountHolderRepo.GetAccountHolderAsync(accountHolderId);

            if (accountHolder == null)
            {
                throw new BaseException.NotFoundException("Account Holder not found!");
            }

            var res = new AccountHolderInfoResponse
            {
                AccountHolderId = accountHolder.Id,
                FullName = accountHolder.FirstName + " " + accountHolder.LastName,
                NRIC = accountHolder.NRIC,
                IsActive = accountHolder.EducationAccount.IsActive,
                SchoolingStatus = accountHolder.SchoolingStatus,
                DOB = accountHolder.DateOfBirth,
                CreatedAt = accountHolder.EducationAccount.CreatedAt,
                Email = accountHolder.Email,
                ContactNumber = accountHolder.ContactNumber,
                Address = accountHolder.Address,
            };
            return res;
        }
    }
}
