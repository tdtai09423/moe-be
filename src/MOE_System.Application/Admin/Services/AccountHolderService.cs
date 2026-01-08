using Microsoft.EntityFrameworkCore;
using MOE_System.Application.Admin.DTOs.AccountHolder;
using MOE_System.Application.Admin.Interfaces;
using MOE_System.Application.Common.Interfaces;
using MOE_System.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static MOE_System.Domain.Common.BaseException;

namespace MOE_System.Application.Admin.Services
{
    public class AccountHolderService : IAccountHolderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordService _passwordService;
        
        public AccountHolderService(IUnitOfWork unitOfWork, IPasswordService passwordService)
        {
            _unitOfWork = unitOfWork;
            _passwordService = passwordService;
        }

        public async Task<AccountHolderDetailResponse> GetAccountHolderDetailAsync(string accountHolderId)
        {
            var accountHolderRepo = _unitOfWork.GetRepository<AccountHolder>();
            
            var accountHolder = await accountHolderRepo.GetByIdAsync(accountHolderId);

            if(accountHolder == null)
            {
                throw new NotFoundException("ACCOUNT_HOLDER_NOT_FOUND", $"Account holder with ID {accountHolderId} not found.");
            }

            var accountHolderDetailResponse = new AccountHolderDetailResponse
            {
                Balance = accountHolder.EducationAccount?.Balance ?? 0,
                ActiveCourseCount = accountHolder.EducationAccount?.Enrollments?.Count ?? 0,
                OutstandingFees = accountHolder.EducationAccount?.Enrollments?
                    .SelectMany(e => e.Invoices)
                    .Where(i => i.Status == "Outstanding") 
                    .Sum(i => i.Amount) ?? 0,
                TotalFeesPaid = accountHolder.EducationAccount?.Enrollments?    
                    .SelectMany(e => e.Invoices)
                    .SelectMany(i => i.Transactions)
                    .Where(t => t.Status == "Completed" || t.Status == "Success")
                    .Sum(t => t.Amount) ?? 0, 
                StudentInformation = new StudentInformation
                {
                    FullName = $"{accountHolder.FirstName} {accountHolder.LastName}",
                    NRIC = accountHolder.NRIC,
                    DateOfBirth = accountHolder.DateOfBirth,
                    Email = accountHolder.Email,
                    ContactNumber = accountHolder.ContactNumber,
                    SchoolingStatus = accountHolder.SchoolingStatus,
                    EducationLevel = accountHolder.EducationLevel,
                    IsActive = !accountHolder.IsDeleted,
                    CreatedAt = accountHolder.CreatedAt
                },
                EnrolledCourses = accountHolder.EducationAccount?.Enrollments?
                    .Select(e => new EnrolledCourseInfo
                    {
                        CourseName = e.CourseOffering?.Course?.CourseName ?? string.Empty,
                        EnrollmentDate = e.EnrollDate,
                        CourseFee = e.CourseOffering?.Course?.FeeAmount ?? 0,
                        Status = e.Status
                    }).ToList() ?? new List<EnrolledCourseInfo>(),
                OutstandingFeesDetails = accountHolder.EducationAccount?.Enrollments?
                    .SelectMany(e => e.Invoices 
                        .Where(i => i.Status == "Outstanding")
                        .Select(i => new OutstandingFeeInfo
                        {
                            CourseName = e.CourseOffering?.Course?.CourseName ?? string.Empty,
                            OutstandingAmount = i.Amount,
                            DueDate = i.DueDate,
                            Status = i.Status
                        }))
                    .ToList() ?? new List<OutstandingFeeInfo>(),
                PaymentHistory = accountHolder.EducationAccount?.Enrollments?
                    .SelectMany(e => e.Invoices)
                    .SelectMany(i => i.Transactions)
                    .Where(t => t.Status == "Completed" || t.Status == "Success")
                    .Select(t => new PaymentHistoryInfo
                    {
                        CourseName = t.Invoice?.Enrollment?.CourseOffering?.Course?.CourseName ?? string.Empty,
                        PaymentDate = t.TransactionAt,
                        AmountPaid = t.Amount,
                        Status = t.Status
                    })
                    .OrderByDescending(p => p.PaymentDate)
                    .ToList() ?? new List<PaymentHistoryInfo>()
            };

            return accountHolderDetailResponse;
        }

        public async Task<PaginatedList<AccountHolderResponse>> GetAccountHoldersAsync(int pageNumber = 1, int pageSize = 20)
        {
            var accountHolderRepo = _unitOfWork.GetRepository<AccountHolder>();
            
            var query = accountHolderRepo.Entities;
            var paginatedAccountHolders = await accountHolderRepo.GetPagging(query, pageNumber, pageSize);
            
            var accountHolderResponses = paginatedAccountHolders.Items.Select(accountHolder => new AccountHolderResponse
            {
                Id = accountHolder.Id,
                FullName = $"{accountHolder.FirstName} {accountHolder.LastName}",
                NRIC = accountHolder.NRIC,
                Age = DateTime.Now.Year - accountHolder.DateOfBirth.Year,
                Balance = accountHolder.EducationAccount?.Balance ?? 0,
                SchoolingStatus = accountHolder.SchoolingStatus,
                EducationLevel = accountHolder.EducationLevel,
                CourseCount = accountHolder.EducationAccount?.Enrollments?.Count ?? 0,
                OutstandingFees = accountHolder.EducationAccount?.Enrollments?
                    .SelectMany(e => e.Invoices)
                    .Where(i => i.Status == "Outstanding")
                    .Sum(i => i.Amount) ?? 0
            }).ToList();

            return new PaginatedList<AccountHolderResponse>(
                accountHolderResponses, 
                paginatedAccountHolders.TotalCount, 
                paginatedAccountHolders.PageIndex, 
                pageSize);
        }

        public async Task<AccountHolderResponse> AddAccountHolderAsync(CreateAccountHolderRequest request)
        {
            var transaction = await _unitOfWork.BeginTransactionAsync();
            
            try
            {
                var accountHolderRepo = _unitOfWork.GetRepository<AccountHolder>();
                var educationAccountRepo = _unitOfWork.GetRepository<EducationAccount>();
                
                var isExistAccountHolder = await accountHolderRepo.Entities.FirstOrDefaultAsync(ah => ah.NRIC == request.NRIC);


                if(isExistAccountHolder != null)
                {
                    await transaction.RollbackAsync();
                    throw new ValidationException("ACCOUNT_HOLDER_EXISTS", $"Account holder with NRIC {request.NRIC} already exists.");
                }

                // Create Account Holder
                var newAccountHolder = new AccountHolder
                {
                    NRIC = request.NRIC,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    DateOfBirth = request.DateOfBirth,
                    Email = request.Email,
                    ContactNumber = request.ContactNumber,
                    SchoolingStatus = "Not in School",
                    CreatedAt = DateTime.UtcNow,
                };
                
                await accountHolderRepo.InsertAsync(newAccountHolder);
                await _unitOfWork.SaveAsync();
                
                // Create Education Account
                var newEducationAccount = new EducationAccount
                {
                    AccountHolderId = newAccountHolder.Id,
                    UserName = request.NRIC,
                    Password = _passwordService.HashPassword(_passwordService.GenerateRandomPassword()),
                    Balance = 0,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                
                await educationAccountRepo.InsertAsync(newEducationAccount);
                await _unitOfWork.SaveAsync();
                
                await transaction.CommitAsync();
                
                return new AccountHolderResponse
                {
                    Id = newAccountHolder.Id,
                    FullName = $"{newAccountHolder.FirstName} {newAccountHolder.LastName}",
                    NRIC = newAccountHolder.NRIC,
                    Age = DateTime.Now.Year - newAccountHolder.DateOfBirth.Year,
                    Balance = newEducationAccount.Balance,
                    SchoolingStatus = newAccountHolder.SchoolingStatus,
                    EducationLevel = newAccountHolder.EducationLevel,
                    CourseCount = 0,
                    OutstandingFees = 0
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
            finally
            {
                await transaction.DisposeAsync();
            }
        }
    }
}
