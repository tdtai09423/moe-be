using Microsoft.EntityFrameworkCore;
using MOE_System.EService.Application.Common.Interfaces;
using MOE_System.EService.Application.DTOs.Dashboard;
using MOE_System.EService.Application.Interfaces.Services;
using MOE_System.EService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MOE_System.EService.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DashboardResponse> GetAccountDashboardAsync(string accountHolderId)
        {
            var accountHolderRepo = _unitOfWork.GetRepository<AccountHolder>();

            var accountHolder = await accountHolderRepo.Entities
                .Where(ah => ah.Id == accountHolderId)
                .Include(ah => ah.EducationAccount)
                .Include(ah => ah.EducationAccount)
                    .ThenInclude(ea => ea!.Enrollments)
                        .ThenInclude(e => e.Invoices)
                .FirstOrDefaultAsync();

            if (accountHolder == null)
            {
                throw new KeyNotFoundException("Account holder not found.");
            }

            var eduAccount = accountHolder.EducationAccount;

            if (eduAccount == null)
            {
                return new DashboardResponse
                {
                    FullName = accountHolder.FullName,
                    Balance = 0,
                    ActiveCoursesCount = 0,
                    OutstandingFees = 0,
                    OutstadingCount = 0,
                    EnrollCourses = new List<EnrollCourse>()
                };
            }

            var enrollments = eduAccount.Enrollments ?? new List<Enrollment>();

            var activeCoursesCount = enrollments.Count(e => e.Status == "Active");

            var outstandingFees = enrollments
                .Sum(e => e.Invoices
                    .Where(i => i.Status == "Outstanding")
                    .Sum(i => i.Amount));

            var outstadingCount = enrollments
                .Count(e => e.Invoices
                    .Any(i => i.Status == "Outstanding"));

            var enrollRepo = _unitOfWork.GetRepository<Enrollment>();

            var rawData = await enrollRepo.Entities
                .Where(e => e.EducationAccountId == eduAccount.Id)
                .Select(e => new
                {
                    e.Course!.CourseName,
                    ProviderName = e.Course.Provider!.Name,
                    e.Course.PaymentType,
                    e.Course.BillingCycle,
                    e.EnrollDate,
                    LatestInvoiceStatus = e.Invoices
                        .OrderByDescending(i => i.DueDate)
                        .Select(i => i.Status)
                        .FirstOrDefault()
                })
                .ToListAsync();

            var enrollCourses = rawData.Select(item =>
            {
                DateOnly billingDateRaw;
                if (item.LatestInvoiceStatus == "Outstanding") 
                {
                    billingDateRaw = new DateOnly(item.EnrollDate.Year, item.EnrollDate.Month, 5);
                }
                else
                {
                    var nextMonth = item.EnrollDate.AddMonths(1);
                    billingDateRaw = new DateOnly(nextMonth.Year, nextMonth.Month, 5);
                }

                return new EnrollCourse
                {
                    CourseName = item.CourseName,
                    ProviderName = item.ProviderName,
                    PaymentType = item.PaymentType,
                    BillingCycle = item.BillingCycle ?? string.Empty,
                    EnrollDate = item.EnrollDate.ToString("dd/MM/yyyy"),
                    BillingDate = billingDateRaw.ToString("dd/MM/yyyy"),
                    PaymentStatus = item.LatestInvoiceStatus ?? "N/A"
                };
            }).ToList();

            return new DashboardResponse
            {
                FullName = accountHolder.FullName,
                Balance = eduAccount.Balance, 
                ActiveCoursesCount = activeCoursesCount,
                OutstandingFees = outstandingFees,
                OutstadingCount = outstadingCount,
                EnrollCourses = enrollCourses
            };
        }
    }
}
