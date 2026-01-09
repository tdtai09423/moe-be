using MOE_System.EService.Application.DTOs;
using MOE_System.EService.Application.Interfaces.Services;
using MOE_System.EService.Application.Interfaces;
using static MOE_System.Domain.Common.BaseException;

namespace MOE_System.EService.Application.Services
{
    public class AccountHolderEServiceService : IAccountHolderEServiceService
    {
        private readonly IAccountHolderRepository _accountHolderRepository;

        public AccountHolderEServiceService(IAccountHolderRepository accountHolderRepository)
        {
            _accountHolderRepository = accountHolderRepository;
        }

        public async Task<AccountHolderInfoResponse> GetAccountHolderInfoAsync(string accountHolderId)
        {
            var accountHolder = await _accountHolderRepository.GetAccountHolderAsync(accountHolderId);

            if (accountHolder == null)
            {
                throw new NotFoundException("ACCOUNT_HOLDER_NOT_FOUND", $"Account holder with ID {accountHolderId} not found.");
            }

            var accountHolderInfoResponse = new AccountHolderInfoResponse
            {
                Id = accountHolder.Id,
                FullName = $"{accountHolder.FirstName} {accountHolder.LastName}",
                NRIC = accountHolder.Nric,
                Email = accountHolder.Email,
                ContactNumber = accountHolder.ContactNumber,
                DateOfBirth = accountHolder.DateOfBirth,
                SchoolingStatus = accountHolder.SchoolingStatus,
                EducationLevel = accountHolder.EducationLevel,
                RegisteredAddress = accountHolder.RegisteredAddress,
                MailingAddress = accountHolder.MailingAddress,
                EducationAccountId = accountHolder.EducationAccount?.Id ?? string.Empty,
                EducationAccountBalance = accountHolder.EducationAccount?.Balance ?? 0,
                IsActive = accountHolder.EducationAccount?.IsActive ?? false,
                CreatedAt = accountHolder.CreatedAt
            };

            return accountHolderInfoResponse;
        }
    }
}
