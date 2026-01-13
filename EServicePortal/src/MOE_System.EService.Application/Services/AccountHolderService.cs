using MOE_System.EService.Application.DTOs;
using MOE_System.EService.Application.Interfaces.Services;
using MOE_System.EService.Application.Interfaces;
using static MOE_System.Domain.Common.BaseException;
using MOE_System.EService.Application.Common.Interfaces;
using MOE_System.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using MOE_System.Domain.Common;

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
            var repo = _unitOfWork.GetRepository<AccountHolder>();

            var accountHolder = await repo.FindAsync(x => x.Id.ToLower() == accountHolderId.ToLower(), 
                    q => q.Include(x => x.EducationAccount)
                );

            if (accountHolder == null)
            {
                throw new BaseException.NotFoundException("This account not found!");
            }

            var accountHolderResponse = new AccountHolderResponse
            {
                Id = accountHolderId,
                FullName = accountHolder.FirstName + " " + accountHolder.LastName,
                NRIC = accountHolder.NRIC,
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
