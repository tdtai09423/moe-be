using MOE_System.EService.Application.DTOs;
using MOE_System.EService.Application.Interfaces.Services;
using MOE_System.EService.Application.Interfaces;
using static MOE_System.Domain.Common.BaseException;
using MOE_System.EService.Application.Common.Interfaces;
using MOE_System.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using MOE_System.Domain.Common;
using Microsoft.IdentityModel.Tokens;

namespace MOE_System.EService.Application.Services
{
    public class AccountHolderService : IAccountHolderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AccountHolderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork; 
        }

        public async Task<AccountHolderResponse> GetAccountHolderAsync(string accountHolderId)
        {
            if (string.IsNullOrWhiteSpace(accountHolderId))
            {
                throw new BaseException.BadRequestException("ID must not be empty or null!");
            }

            var repo = _unitOfWork.GetRepository<AccountHolder>();

            var accountHolder = await repo.Entities.AsNoTracking()
                .Include(a => a.EducationAccount)
                .Where(a => a.Id == accountHolderId)
                .FirstOrDefaultAsync();

            if (accountHolder == null)
            {
                throw new BaseException.NotFoundException("This account is not found!");
            }

            var accountHolderResponse = new AccountHolderResponse
            {
                Id = accountHolderId,
                FullName = accountHolder.FirstName + " " + accountHolder.LastName,
                NRIC = accountHolder.Nric,
                Email = accountHolder.Email,
                ContactNumber = accountHolder.ContactNumber,
                DateOfBirth = accountHolder.DateOfBirth,
                SchoolingStatus = accountHolder.SchoolingStatus,    
                EducationLevel = accountHolder.EducationLevel,
                RegisteredAddress = accountHolder.RegisteredAddress,
                MailingAddress = accountHolder.MailingAddress,
                EducationAccountId = accountHolder.EducationAccount?.Id ?? "",
                EducationAccountBalance = accountHolder.EducationAccount?.Balance ?? 0,
                IsActive = accountHolder.EducationAccount?.IsActive ?? false,
                CreatedAt = accountHolder.EducationAccount?.CreatedAt ?? DateTime.Now,
            };

            return accountHolderResponse;
        }
    }
}
