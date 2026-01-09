using Microsoft.EntityFrameworkCore;
using MOE_System.Application.Common.Interfaces;
using MOE_System.Application.DTOs;
using MOE_System.Application.Interfaces;
using MOE_System.Domain.Entities;

namespace MOE_System.Application.Services;

public class EducationAccountService : IEducationAccountService
{
    private readonly IUnitOfWork _unitOfWork;

    public EducationAccountService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<AccountBalanceDto?> GetAccountBalanceAsync(string accountId)
    {
        var account = await _unitOfWork.GetRepository<EducationAccount>()
            .Entities
            .Where(e => e.Id == accountId && e.DeletedAt == null)
            .Select(e => new AccountBalanceDto
            {
                AccountId = e.Id,
                Balance = e.Balance,
                IsActive = e.IsActive,
                LastLoginAt = e.LastLoginAt
            })
            .FirstOrDefaultAsync();

        return account;
    }

    public async Task<OutstandingFeeDto?> GetOutstandingFeesAsync(string accountId)
    {
        var account = await _unitOfWork.GetRepository<EducationAccount>()
            .Entities
            .Where(e => e.Id == accountId && e.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (account == null)
        {
            return null;
        }

        var outstandingInvoices = await _unitOfWork.GetRepository<Invoice>()
            .Entities
            .Include(i => i.Enrollment)
                .ThenInclude(e => e!.CourseOffering)
                .ThenInclude(co => co!.Course)
            .Where(i => i.Enrollment!.EducationAccountId == accountId 
                        && (i.Status == "Pending" || i.Status == "Overdue"))
            .Select(i => new InvoiceDetailDto
            {
                InvoiceId = i.Id,
                CourseName = i.Enrollment!.CourseOffering!.Course!.CourseName,
                Amount = i.Amount,
                DueDate = i.DueDate,
                Status = i.Status
            })
            .ToListAsync();

        return new OutstandingFeeDto
        {
            AccountId = accountId,
            TotalOutstandingAmount = outstandingInvoices.Sum(i => i.Amount),
            OutstandingInvoices = outstandingInvoices
        };
    }
}
