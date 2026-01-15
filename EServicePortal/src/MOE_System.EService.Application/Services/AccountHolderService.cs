using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MOE_System.EService.Application.Common.Interfaces;
using MOE_System.EService.Application.DTOs.AccountHolder;
using MOE_System.EService.Application.DTOs.Dashboard;
using MOE_System.EService.Application.Interfaces;
using MOE_System.EService.Application.Interfaces.Services;
using MOE_System.EService.Domain.Common;
using MOE_System.EService.Domain.Entities;
using static MOE_System.EService.Domain.Common.BaseException;

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

        public async Task<YourCourseResponse> GetYourCoursesAsync(string accountHolderId)
        {
            var accountHolderRepo = _unitOfWork.GetRepository<AccountHolder>();

            var accountHolder = await accountHolderRepo.Entities
                .Where(ah => ah.Id == accountHolderId)
                .Include(ah => ah.EducationAccount)
                    .ThenInclude(ea => ea!.Enrollments)
                        .ThenInclude(e => e.Course!)
                            .ThenInclude(c => c.Provider)
                .Include(ah => ah.EducationAccount)
                    .ThenInclude(ea => ea!.Enrollments)
                        .ThenInclude(e => e.Invoices)
                            .ThenInclude(i => i.Transactions)
                .FirstOrDefaultAsync();

            if (accountHolder?.EducationAccount == null)
            {
                return new YourCourseResponse();
            }

            var eduAccount = accountHolder.EducationAccount;

            var enrollments = accountHolder.EducationAccount.Enrollments ?? new List<Enrollment>();

            var allInvoices = enrollments.SelectMany(e => (e.Invoices ?? new List<Invoice>()).Select(i => new
            {
                Invoice = i,
                e.Course,
                Enrollment = e
            })).ToList();

            var enrollRepo = _unitOfWork.GetRepository<Enrollment>();

            var rawData = await enrollRepo.Entities
                .Where(e => e.EducationAccountId == eduAccount.Id)
                .Select(e => new
                {
                    e.Course!.CourseName,
                    ProviderName = e.Course.Provider!.Name,
                    e.Course.FeeAmount,
                    e.Course.BillingCycle,
                    e.EnrollDate,
                    LatestInvoiceStatus = e.Invoices
                        .OrderByDescending(i => i.DueDate)
                        .Select(i => i.Status)
                        .FirstOrDefault()
                })
                .ToListAsync();

            var listEnrolledCourses = rawData.Select(item =>
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

                return new EnrolledCourse
                {
                    CourseName = item.CourseName,
                    ProviderName = item.ProviderName,
                    CourseFee = item.FeeAmount,
                    BillingCycle = item.BillingCycle ?? string.Empty,
                    EnrolledDate = item.EnrollDate.ToString("dd/MM/yyyy"),
                    BillingDate = billingDateRaw.ToString("dd/MM/yyyy"),
                    PaymentStatus = item.LatestInvoiceStatus ?? "N/A"
                };
            }).ToList();

            var outstandingFees = allInvoices
                .Where(x => x.Invoice.Status == "Outstanding")
                .Sum(x => x.Invoice.Amount);

            var currentBalance = eduAccount.Balance;

            var listPendingFees = allInvoices
                .Where(x => x.Invoice.Status == "Outstanding")
                .Select(x => new PendingFees
                {
                    CourseName = x.Course?.CourseName ?? "Unknown",
                    ProviderName = x.Course?.Provider?.Name,
                    AmountDue = x.Invoice.Amount,
                    BillingCycle = x.Course?.BillingCycle ?? "N/A",
                    BillingDate = x.Invoice.Status == "Outstanding"
                        ? new DateOnly(x.Invoice.DueDate.Year, x.Invoice.DueDate.Month, 5).ToString("dd/MM/yyyy")
                        : "N/A",
                    DueDate = x.Invoice.DueDate.ToString("dd/MM/yyyy"),
                    PaymentStatus = x.Invoice.Status
                }).ToList();

            var listPaymentHistory = allInvoices
                .Where(x => x.Invoice.Status == "Paid")
                .Select(x => new PaymentHistory
                {
                    CourseName = x.Course?.CourseName ?? "Unknown",
                    ProviderName = x.Course?.Provider?.Name,
                    AmountPaid = x.Invoice.Amount,
                    BillingCycle = x.Course?.BillingCycle ?? "N/A",
                    PaymentDate = x.Invoice.Transactions?.Where(t => t.InvoiceId == x.Invoice.Id && t.Status == "Success")?
                        .OrderByDescending(t => t.TransactionAt)
                        .Select(t => t.TransactionAt.ToString("dd/MM/yyyy"))
                        .FirstOrDefault() ?? "N/A",
                    PaymentMethod = x.Invoice.Transactions?.Where(t => t.InvoiceId == x.Invoice.Id && t.Status == "Success")?
                        .OrderByDescending(t => t.TransactionAt)
                        .Select(t => t.PaymentMethod)
                        .FirstOrDefault() ?? "N/A"
                }).OrderByDescending(h => h.PaymentDate)
                .ToList();

            // 9. Trả về kết quả cuối cùng
            return new YourCourseResponse
            {
                OutstandingFees = outstandingFees,
                Balance = currentBalance,
                EnrolledCourses = listEnrolledCourses,
                PendingFees = listPendingFees,
                PaymentHistory = listPaymentHistory
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
