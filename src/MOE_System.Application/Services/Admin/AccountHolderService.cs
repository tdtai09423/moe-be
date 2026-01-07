using MOE_System.Application.DTOs.AccountHolder;
using MOE_System.Application.Interfaces;
using MOE_System.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MOE_System.Application.Services.Admin
{
    public class AccountHolderService : IAccountHolderService
    {
        private readonly IUnitOfWork _unitOfWork;
        public AccountHolderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<AccountHolderDetailResponse> GetAccountHolderDetailAsync(int accountHolderId)
        {
            try
            {
                var accountHolderRepo = _unitOfWork.GetRepository<AccountHolder>();
                
                var accountHolder = await accountHolderRepo.GetByIdAsync(accountHolderId);

                if(accountHolder == null)
                {
                    throw new KeyNotFoundException("Account holder not found.");
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
            catch 
            {
                _unitOfWork.RollBack();
                _unitOfWork.Dispose();
                throw;
            }
        }

        public async Task<List<AccountHolderResponse>> GetAccountHoldersAsync()
        {
            try
            {
                var accountHolderRepo = _unitOfWork.GetRepository<AccountHolder>();

                var accountHolders = await accountHolderRepo.GetAllAsync();
                var accountHolderResponses = new List<AccountHolderResponse>();

                foreach (var accountHolder in accountHolders)
                {
                    accountHolderResponses.Add(new AccountHolderResponse
                    {
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
                          .Sum(i => i.Amount) ?? 0,
                    });
                }

                return accountHolderResponses;
            }
            catch
            {
                _unitOfWork.RollBack();
                _unitOfWork.Dispose();
                throw;
            }
        }
    }
}
