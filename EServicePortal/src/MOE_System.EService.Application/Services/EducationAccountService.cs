using MOE_System.EService.Application.DTOs;
using MOE_System.EService.Application.Interfaces.Services;
using MOE_System.EService.Application.Interfaces;
using static MOE_System.Domain.Common.BaseException;

namespace MOE_System.EService.Application.Services
{
    public class EducationAccountService : IEducationAccountService
    {
        private readonly IEducationAccountRepository _educationAccountRepository;

        public EducationAccountService(IEducationAccountRepository educationAccountRepository)
        {
            _educationAccountRepository = educationAccountRepository;
        }

        public async Task<EducationAccountBalanceResponse> GetEducationAccountBalanceAsync(string educationAccountId)
        {
            var educationAccount = await _educationAccountRepository.GetEducationAccountAsync(educationAccountId);

            if (educationAccount == null)
            {
                throw new NotFoundException("EDUCATION_ACCOUNT_NOT_FOUND", $"Education account with ID {educationAccountId} not found.");
            }

            var response = new EducationAccountBalanceResponse
            {
                EducationAccountId = educationAccount.Id,
                AccountHolderId = educationAccount.AccountHolderId,
                Balance = educationAccount.Balance,
                IsActive = educationAccount.IsActive,
                LastUpdated = educationAccount.UpdatedAt ?? educationAccount.CreatedAt
            };

            return response;
        }
    }
}
