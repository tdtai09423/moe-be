using MOE_System.EService.Application.DTOs;
using MOE_System.EService.Application.Interfaces.Services;
using MOE_System.EService.Application.Interfaces;
using MOE_System.EService.Application.Common.Interfaces;
using MOE_System.EService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using MOE_System.EService.Domain.Common;
using System.Net.WebSockets;
using static MOE_System.EService.Domain.Common.BaseException;

namespace MOE_System.EService.Application.Services
{
    public class EducationAccountService : IEducationAccountService
    {
        private readonly IUnitOfWork _unitOfWork;

        public EducationAccountService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BalanceResponse> GetBalanceAsync(string educationAccountId)
        {
            if (string.IsNullOrWhiteSpace(educationAccountId))
            {
                throw new BaseException.BadRequestException("ID must not be empty or null!");
            }

            var repo = _unitOfWork.GetRepository<EducationAccount>();

            var educationAccount = await repo.GetByIdAsync(educationAccountId);

            if (educationAccount == null)
            {
                throw new BaseException.NotFoundException("This education account is not found!");
            }

            var result = new BalanceResponse
            {
                EducationAccountId = educationAccountId,
                AccountHolderId = educationAccount.AccountHolderId,
                Balance = educationAccount.Balance,
                IsActive = educationAccount.IsActive,
                LastUpdated = educationAccount.UpdatedAt,
            };

            return result;
        }

        public async Task<OutstandingFeeResponse> GetOutstandingFeeAsync(string educationAccountId)
        {
            if (string.IsNullOrWhiteSpace(educationAccountId))
            {
                throw new BaseException.BadRequestException("ID must not be empty or null!");
            }

            var educationAccountRepo = _unitOfWork.GetRepository<EducationAccount>();

            var educationAccount = await educationAccountRepo.GetByIdAsync(educationAccountId);

            if (educationAccount == null)
            {
                throw new BaseException.NotFoundException("This education account is not found!");
            }

            var enrollmentRepo = _unitOfWork.GetRepository<Enrollment>();

            var activeCourses = await enrollmentRepo.Entities
                .Include(c => c.EducationAccount).ThenInclude(e => e.AccountHolder)
                .Include(e => e.Invoices).ThenInclude(i => i.Transactions)
                .Where(e => e.EducationAccountId == educationAccountId)
                .ToListAsync();

            // ---- Calculate Outstanding fee ---
            var allInvoices = activeCourses.SelectMany(a => a.Invoices).ToList();

            var totalInvoicedAmount = allInvoices.Sum(a => a.Amount);

            var totalPaidAmount = allInvoices.SelectMany(i => i.Transactions)
                .Where(t => t.Status == "Success")
                .Sum(t => t.Amount);

            var outstaningAmount = totalInvoicedAmount - totalPaidAmount;
            // ----------------------------------

            // --- GET INVOICES ---
            var outstandingInvoiceInfos = new List<OutstandingInvoiceInfo>();

            foreach(var invoice in allInvoices)
            {
                var invoiceResponse = new OutstandingInvoiceInfo
                {
                    InvoiceId = invoice.Id,
                    EnrollmentId = invoice.EnrollmentID,
                    CourseName = invoice.Enrollment?.Course?.CourseName ?? "",
                    Amount = invoice.Amount,
                    DueDate = invoice.DueDate,
                    Status = invoice.Status,
                };
                outstandingInvoiceInfos.Add(invoiceResponse);
            }

            var result = new OutstandingFeeResponse
            {
                EducationAccountId = educationAccountId,
                AccountHolderId = educationAccount.AccountHolderId,
                TotalOutstandingFee = outstaningAmount,
                OutstandingInvoices = outstandingInvoiceInfos,
            };
            
            return result;
        }
    }
}
