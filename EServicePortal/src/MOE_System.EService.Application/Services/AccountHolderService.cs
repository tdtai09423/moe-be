using MOE_System.EService.Application.DTOs;
using MOE_System.EService.Application.Interfaces.Services;
using MOE_System.EService.Application.Interfaces;
using static MOE_System.EService.Domain.Common.BaseException;
using MOE_System.EService.Application.Common.Interfaces;
using MOE_System.EService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MOE_System.EService.Domain.Common;

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

        public async Task<AccountHolderProfileResponse> GetMyProfileAsync(string accountHolderId)
        {
            var repo = _unitOfWork.GetRepository<AccountHolder>();

            var accountHolder = await repo.FindAsync(
                x => x.Id.ToLower() == accountHolderId.ToLower(),
                q => q.Include(x => x.EducationAccount)
            );

            if (accountHolder == null)
            {
                throw new BaseException.NotFoundException("Account holder not found!");
            }

            return new AccountHolderProfileResponse
            {
                FullName = $"{accountHolder.FirstName} {accountHolder.LastName}",
                NRIC = accountHolder.NRIC,
                DateOfBirth = accountHolder.DateOfBirth,
                AccountCreated = accountHolder.EducationAccount?.CreatedAt ?? accountHolder.CreatedAt,
                SchoolingStatus = accountHolder.SchoolingStatus,
                EducationLevel = accountHolder.EducationLevel,
                ResidentialStatus = accountHolder.ResidentialStatus,
                EmailAddress = accountHolder.Email,
                PhoneNumber = accountHolder.ContactNumber,
                RegisteredAddress = accountHolder.RegisteredAddress,
                MailingAddress = accountHolder.MailingAddress
            };
        }

        public async Task<UpdateProfileResponse> UpdateProfileAsync(string accountHolderId, UpdateProfileRequest request)
        {
            var accountHolderRepo = _unitOfWork.GetRepository<AccountHolder>();
            var accountHolder = await accountHolderRepo.FindAsync(x => x.Id.ToLower() == accountHolderId.ToLower());
            if (accountHolder == null)
            {
                throw new BaseException.NotFoundException("Account holder not found");
            }

            accountHolder.Email = !string.IsNullOrWhiteSpace(request.Email)
                          ? request.Email
                          : accountHolder.Email;

            accountHolder.ContactNumber = !string.IsNullOrWhiteSpace(request.ContactNumber)
                                          ? request.ContactNumber
                                          : accountHolder.ContactNumber;

            accountHolder.MailingAddress = !string.IsNullOrWhiteSpace(request.MailingAddress)
                                           ? request.MailingAddress
                                           : accountHolder.MailingAddress;

            accountHolder.RegisteredAddress = !string.IsNullOrWhiteSpace(request.RegisteredAddress)
                                              ? request.RegisteredAddress
                                              : accountHolder.RegisteredAddress;

            accountHolderRepo.Update(accountHolder);
            await _unitOfWork.SaveAsync();
            return new UpdateProfileResponse
            {
                AccountHolderId = accountHolder.Id,
                FullName = $"{accountHolder.FirstName} {accountHolder.LastName}",
                Email = accountHolder.Email,
                ContactNumber = accountHolder.ContactNumber,
                MailingAddress = accountHolder.MailingAddress,
                RegisteredAddress = accountHolder.RegisteredAddress
            };
        }
    }
}
