using Microsoft.EntityFrameworkCore;
using MOE_System.Application.Common;
using MOE_System.Application.Common.Interfaces;
using MOE_System.Application.DTOs;
using MOE_System.Application.DTOs.AccountHolder;
using MOE_System.Application.DTOs.AccountHolder.Request;
using MOE_System.Application.DTOs.AccountHolder.Response;
using MOE_System.Application.Interfaces;
using MOE_System.Domain.Entities;
using MOE_System.Domain.Enums;
using System.Text.RegularExpressions;
using System.Linq;
using static MOE_System.Domain.Common.BaseException;

namespace MOE_System.Application.Services;

public class AccountHolderService : IAccountHolderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordService _passwordService;

    public AccountHolderService(IUnitOfWork unitOfWork, IPasswordService passwordService)
    {
        _unitOfWork = unitOfWork;
        _passwordService = passwordService;
    }

    public async Task<ResidentInfoResponse> GetResidentAccountHolderByNRICAsync(string nric)
    {
        var residentRepo = _unitOfWork.GetRepository<Resident>();
        
        var resident = await residentRepo.Entities
            .FirstOrDefaultAsync(ah => ah.NRIC == nric);
        if(resident == null)
        {
            throw new NotFoundException("RESIDENT_NOT_FOUND", $"Account holder with NRIC {nric} not found.");
        }
        return new ResidentInfoResponse
        {
            FullName = resident.PrincipalName,
            DateOfBirth = resident.DateOfBirth,
            Email = resident.EmailAddress,
            PhoneNumber = resident.MobileNumber,
            RegisteredAddress = resident.RegisteredAddress,
            ResidentialStatus = resident.ResidentialStatus
        };
    }

    public async Task<AccountHolderDetailResponse> GetAccountHolderDetailAsync(string accountHolderId)
    {
        var accountHolderRepo = _unitOfWork.GetRepository<AccountHolder>();
        
        var accountHolder = await accountHolderRepo.Entities
            .Include(ah => ah.EducationAccount!)
                .ThenInclude(ea => ea.Enrollments)
                    .ThenInclude(en => en.Course!)
                        .ThenInclude(c => c.Provider)
            .Include(ah => ah.EducationAccount!)
                .ThenInclude(ea => ea.Enrollments)
                    .ThenInclude(en => en.Invoices)
            .Include(ah => ah.EducationAccount!)
                .ThenInclude(ea => ea.HistoryOfChanges)
            .FirstOrDefaultAsync(ah => ah.Id == accountHolderId);

        if (accountHolder == null)
        {
            throw new NotFoundException("ACCOUNT_HOLDER_NOT_FOUND", $"Account holder with ID {accountHolderId} not found.");
        }

        var accountHolderDetailResponse = new AccountHolderDetailResponse
        {
            FullName = $"{accountHolder.FirstName} {accountHolder.LastName}",
            NRIC = accountHolder.NRIC,
            Balance = accountHolder.EducationAccount?.Balance ?? 0,
            CourseCount = accountHolder.EducationAccount?.Enrollments?.Count ?? 0,
            OutstandingFees = accountHolder.EducationAccount?.Enrollments?
                .SelectMany(e => e.Invoices)
                .Where(i => i.Status == "Outstanding")
                .Sum(i => i.Amount) ?? 0,
            IsActive = accountHolder.EducationAccount?.IsActive,

            StudentInformation = new StudentInformation
            {
                DateOfBirth = accountHolder.DateOfBirth.ToString("dd/MM/yyyy"),
                Email = accountHolder.Email,
                ContactNumber = accountHolder.ContactNumber,
                SchoolingStatus = accountHolder.SchoolingStatus,
                EducationLevel = accountHolder.EducationLevel,
                ResidentialStatus = accountHolder.ResidentialStatus,
                RegisteredAddress = accountHolder.RegisteredAddress,
                MailingAddress = accountHolder.MailingAddress,
                CreatedAt = accountHolder.CreatedAt.ToString("dd/MM/yyyy")
            },

            EnrolledCourses = accountHolder.EducationAccount?.Enrollments?
                .Select(e => new EnrolledCourseInfo
                {
                    CourseName = e.Course?.CourseName ?? string.Empty,
                    ProviderName = e.Course?.Provider?.Name ?? string.Empty,
                    BillingCycle = e.Course?.BillingCycle ?? string.Empty,
                    TotalFree = e.Course?.FeeAmount ?? 0,
                    EnrollmentDate = e.EnrollDate.ToString("dd/MM/yyyy"),
                    CollectedFee = e.Invoices?.Where(i => i.Status == "Paid").Sum(i => i.Amount) ?? 0,
                    NextPaymentDue = e.Invoices?
                        .Where(i => i.Status == "Outstanding")
                        .OrderBy(i => i.DueDate)
                        .Select(i => i.DueDate.ToString("dd/mm/yyyy"))
                        .FirstOrDefault() ?? "N/A",
                    PaymentStatus = e.Invoices != null && e.Invoices.Any(i => i.Status == "Outstanding") ? "Pending" : "Up to Date"

                }).ToList() ?? new List<EnrolledCourseInfo>(),

            OutstandingFeesDetails = accountHolder.EducationAccount?.Enrollments?
                .SelectMany(e => e.Invoices
                    .Where(i => i.Status == "Outstanding")
                    .Select(i => new OutstandingFeeInfo
                    {
                        CourseName = e.Course?.CourseName ?? string.Empty,
                        ProviderName = e.Course?.Provider?.Name ?? string.Empty,
                        OutstandingAmount = i.Amount,
                        DueDate = i.DueDate.ToString("dd/MM/yyyy"),
                        BillingDate = new DateTime(i.DueDate.Year, i.DueDate.Month, 1).AddDays(4).ToString("dd/MM/yyyy")
                    }))
                .ToList() ?? new List<OutstandingFeeInfo>(),

            TopUpHistory = accountHolder.EducationAccount?.HistoryOfChanges?.Where(h => h.Type == "TopUp")
                .OrderByDescending(h => h.CreatedAt)
                .Select(h => new TopUpHistoryInfo
                {
                    TopUpDate = h.CreatedAt.ToString("dd/MM/yyyy"),
                    TopUpTime = h.CreatedAt.ToString("HH:mm tt"),
                    Amount = h.Amount,
                    Reference = string.Empty,
                    Description = string.Empty
                })
                .ToList() ?? new List<TopUpHistoryInfo>(),

            PaymentHistory = accountHolder.EducationAccount?.Enrollments?
                .SelectMany(e => e.Invoices)
                .SelectMany(i => i.Transactions)
                .Where(t => t.Status == "Completed" || t.Status == "Success")
                .Select(t => new PaymentHistoryInfo
                {
                    CourseName = t.Invoice?.Enrollment?.Course?.CourseName ?? string.Empty,
                    ProviderName = t.Invoice?.Enrollment?.Course?.Provider?.Name ?? string.Empty,
                    PaymentDate = t.TransactionAt.ToString("dd/MM/yyyy"),
                    AmountPaid = t.Amount,
                    PaymentMethod = t.PaymentMethod
                })
                .OrderByDescending(p => p.PaymentDate)
                .ToList() ?? new List<PaymentHistoryInfo>()
        };

        return accountHolderDetailResponse;
    }

    public async Task<PaginatedList<AccountHolderResponse>> GetAccountHoldersAsync(int pageNumber = 1, int pageSize = 20, AccountHolderFilterParams? filters = null)
    {
        var accountHolderRepo = _unitOfWork.GetRepository<AccountHolder>();
        
        var query = accountHolderRepo.Entities.AsQueryable();

        // Apply filters and sorting via helper methods
        query = ApplyFilters(query, filters);
        query = ApplySorting(query, filters);

        query = query.Include(ah => ah.EducationAccount!)
                     .ThenInclude(ea => ea.Enrollments); 

        var paginatedAccountHolders = await accountHolderRepo.GetPagging(query, pageNumber, pageSize);
        
        var accountHolderResponses = paginatedAccountHolders.Items.Select(accountHolder => new AccountHolderResponse
        {
            Id = accountHolder.Id,
            FullName = $"{accountHolder.FirstName} {accountHolder.LastName}",
            NRIC = accountHolder.NRIC,
            Age = DateTime.Now.Year - accountHolder.DateOfBirth.Year,
            Balance = accountHolder.EducationAccount?.Balance ?? 0,
            EducationLevel = accountHolder.EducationLevel,
            ResidentialStatus = accountHolder.ResidentialStatus,
            CreatedDate = DateOnly.FromDateTime(accountHolder.CreatedAt).ToString("dd/MM/yyyy"),
            CreateTime = accountHolder.CreatedAt.ToString("HH:mm tt"),
            CourseCount = accountHolder.EducationAccount?.Enrollments?.Count ?? 0,
        }).ToList();

        return new PaginatedList<AccountHolderResponse>(
            accountHolderResponses, 
            paginatedAccountHolders.TotalCount, 
            paginatedAccountHolders.PageIndex, 
            pageSize);
    }

    // Extracted filter logic
    private IQueryable<AccountHolder> ApplyFilters(IQueryable<AccountHolder> query, AccountHolderFilterParams? filters)
    {
        if (filters == null) return query;

        query = query.Where(ah => ah.EducationAccount != null && ah.EducationAccount.IsActive == filters.IsActive);

        if (!string.IsNullOrWhiteSpace(filters.Search))
        {
            var s = filters.Search.Trim().ToLower();
            query = query.Where(ah => (ah.FirstName + " " + ah.LastName).ToLower().Contains(s)
                                       || ah.NRIC.ToLower().Contains(s)
                                       || ah.Email.ToLower().Contains(s)
                                       || ah.ContactNumber.ToLower().Contains(s));
        }

        if (filters.EducationLevels != null && filters.EducationLevels.Any())
        {
            var allowed = filters.EducationLevels.Select(e => e.ToString().ToLower()).ToList();
            var allowedFriendly = filters.EducationLevels.Select(e => e.ToFriendlyString().ToLower()).ToList();

            query = query.Where(ah => ah.EducationLevel != null && (
                allowed.Contains(ah.EducationLevel.ToLower()) || allowedFriendly.Contains(ah.EducationLevel.ToLower())
            ));
        }

        if (!string.IsNullOrWhiteSpace(filters.SchoolingStatus))
        {
            var ss = filters.SchoolingStatus.Trim().ToLower();
            query = query.Where(ah => ah.SchoolingStatus != null && ah.SchoolingStatus.ToLower() == ss);
        }

        if (filters.ResidentialStatuses != null && filters.ResidentialStatuses.Any())
        {
            var allowedRes = filters.ResidentialStatuses.Select(r => r.ToString().ToLower()).ToList();
            var allowedResFriendly = filters.ResidentialStatuses.Select(r => r.ToFriendlyString().ToLower()).ToList();

           query = query.Where(ah => ah.ResidentialStatus != null && (
                allowedRes.Contains(ah.ResidentialStatus.ToLower()) || allowedResFriendly.Contains(ah.ResidentialStatus.ToLower())
            ));
        }

        if (filters.MinBalance.HasValue)
        {
            var min = filters.MinBalance.Value;
            query = query.Where(ah => ah.EducationAccount != null && ah.EducationAccount.Balance >= min);
        }

        if (filters.MaxBalance.HasValue)
        {
            var max = filters.MaxBalance.Value;
            query = query.Where(ah => ah.EducationAccount != null && ah.EducationAccount.Balance <= max);
        }

        if (filters.MinAge.HasValue || filters.MaxAge.HasValue)
        {
            var today = DateTime.Today;

            if (filters.MinAge.HasValue)
            {
                var maxDob = today.AddYears(-filters.MinAge.Value);
                query = query.Where(ah => ah.DateOfBirth <= maxDob);
            }

            if (filters.MaxAge.HasValue)
            {
                var minDob = today.AddYears(-filters.MaxAge.Value);
                query = query.Where(ah => ah.DateOfBirth >= minDob);
            }
        }

        return query;
    }

    // Extracted sorting logic
    private IQueryable<AccountHolder> ApplySorting(IQueryable<AccountHolder> query, AccountHolderFilterParams? filters)
    {
        if (filters == null || !filters.SortBy.HasValue) return query;

        var desc = filters.SortDescending == true;
        switch (filters.SortBy.Value)
        {
            case SortBy.FullName:
                query = desc
                    ? query.OrderByDescending(ah => (ah.FirstName + " " + ah.LastName).ToLower())
                    : query.OrderBy(ah => (ah.FirstName + " " + ah.LastName).ToLower());
                break;
            case SortBy.Age:
                query = desc
                    ? query.OrderBy(ah => ah.DateOfBirth)
                    : query.OrderByDescending(ah => ah.DateOfBirth);
                break;
            case SortBy.Balance:
                query = desc
                    ? query.OrderByDescending(ah => ah.EducationAccount != null ? ah.EducationAccount.Balance : 0)
                    : query.OrderBy(ah => ah.EducationAccount != null ? ah.EducationAccount.Balance : 0);
                break;
            case SortBy.EducationLevel:
                var primaryName = EducationLevel.Primary.ToString().ToLower();
                var secondaryName = EducationLevel.Secondary.ToString().ToLower();
                var postSecondaryName = EducationLevel.PostSecondary.ToString().ToLower();
                var tertiaryName = EducationLevel.Tertiary.ToString().ToLower();
                var postGraduateName = EducationLevel.PostGraduate.ToString().ToLower();

                if (desc)
                {
                    query = query.OrderByDescending(ah => ah.EducationLevel != null ? (
                        ah.EducationLevel.ToLower() == primaryName || ah.EducationLevel.ToLower() == "primary" ? 0 :
                        ah.EducationLevel.ToLower() == secondaryName || ah.EducationLevel.ToLower() == "secondary" ? 1 :
                        (ah.EducationLevel.ToLower() == postSecondaryName || ah.EducationLevel.ToLower() == "post-secondary" || ah.EducationLevel.ToLower() == "post secondary" || ah.EducationLevel.ToLower() == "postsecondary") ? 2 :
                        ah.EducationLevel.ToLower() == tertiaryName || ah.EducationLevel.ToLower() == "tertiary" ? 3 :
                        (ah.EducationLevel.ToLower() == postGraduateName || ah.EducationLevel.ToLower() == "post-graduate" || ah.EducationLevel.ToLower() == "post graduate" || ah.EducationLevel.ToLower() == "postgraduate") ? 4 : 99
                    ) : 99);
                }
                else
                {
                    query = query.OrderBy(ah => ah.EducationLevel != null ? (
                        ah.EducationLevel.ToLower() == primaryName || ah.EducationLevel.ToLower() == "primary" ? 0 :
                        ah.EducationLevel.ToLower() == secondaryName || ah.EducationLevel.ToLower() == "secondary" ? 1 :
                        (ah.EducationLevel.ToLower() == postSecondaryName || ah.EducationLevel.ToLower() == "post-secondary" || ah.EducationLevel.ToLower() == "post secondary" || ah.EducationLevel.ToLower() == "postsecondary") ? 2 :
                        ah.EducationLevel.ToLower() == tertiaryName || ah.EducationLevel.ToLower() == "tertiary" ? 3 :
                        (ah.EducationLevel.ToLower() == postGraduateName || ah.EducationLevel.ToLower() == "post-graduate" || ah.EducationLevel.ToLower() == "post graduate" || ah.EducationLevel.ToLower() == "postgraduate") ? 4 : 99
                    ) : 99);
                }

                break;
            case SortBy.CreatedDate:
                query = desc
                    ? query.OrderByDescending(ah => ah.CreatedAt)
                    : query.OrderBy(ah => ah.CreatedAt);
                break;
            default:
                break;
        }

        return query;
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

            string pattern = @"^([^\s]+)\s+(.*)$";
            Match match = Regex.Match(request.FullName, pattern);

            string firstName = string.Empty;
            string lastName = string.Empty;

            if (match.Success)
            {
                firstName = match.Groups[1].Value;
                lastName = match.Groups[2].Value;
            }

            // Create Account Holder
            var newAccountHolder = new AccountHolder
            {
                NRIC = request.NRIC,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = request.DateOfBirth,
                Email = request.Email,
                ContactNumber = request.ContactNumber,
                EducationLevel = request.EducationLevel,
                RegisteredAddress = request.RegisteredAddress,
                MailingAddress = request.MailingAddress,
                SchoolingStatus = SchoolingStatus.NotInSchool.ToFriendlyString(),
                ResidentialStatus = request.ResidentialStatus,
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
                EducationLevel = newAccountHolder.EducationLevel,
                CreatedDate = DateOnly.FromDateTime(newAccountHolder.CreatedAt).ToString("dd/MM/yyyy"),
                CreateTime = newAccountHolder.CreatedAt.ToString("HH:mm tt"),
                ResidentialStatus = newAccountHolder.ResidentialStatus,
                CourseCount = 0,
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

    public async Task<BulkAccountOperationResponse> ActivateAccountsAsync(BulkAccountOperationRequest request)
    {
        var response = new BulkAccountOperationResponse
        {
            TotalProcessed = request.AccountIds.Count
        };

        var educationAccountRepo = _unitOfWork.GetRepository<EducationAccount>();

        foreach (var accountId in request.AccountIds)
        {
            try
            {
                var educationAccount = await educationAccountRepo.GetByIdAsync(accountId);

                if (educationAccount == null)
                {
                    response.FailedOperations.Add(new FailedAccountOperation
                    {
                        AccountId = accountId,
                        Reason = "Education account not found"
                    });
                    response.FailedCount++;
                    continue;
                }

                if (educationAccount.IsActive)
                {
                    response.FailedOperations.Add(new FailedAccountOperation
                    {
                        AccountId = accountId,
                        Reason = "Account is already active"
                    });
                    response.FailedCount++;
                    continue;
                }

                educationAccount.IsActive = true;
                educationAccount.UpdatedAt = DateTime.UtcNow;
                
                await educationAccountRepo.UpdateAsync(educationAccount);
                response.SuccessfulIds.Add(accountId);
                response.SuccessCount++;
            }
            catch (Exception ex)
            {
                response.FailedOperations.Add(new FailedAccountOperation
                {
                    AccountId = accountId,
                    Reason = $"Error: {ex.Message}"
                });
                response.FailedCount++;
            }
        }

        await _unitOfWork.SaveAsync();
        return response;
    }

    public async Task<BulkAccountOperationResponse> DeactivateAccountsAsync(BulkAccountOperationRequest request)
    {
        var response = new BulkAccountOperationResponse
        {
            TotalProcessed = request.AccountIds.Count
        };

        var educationAccountRepo = _unitOfWork.GetRepository<EducationAccount>();

        foreach (var accountId in request.AccountIds)
        {
            try
            {
                var educationAccount = await educationAccountRepo.GetByIdAsync(accountId);

                if (educationAccount == null)
                {
                    response.FailedOperations.Add(new FailedAccountOperation
                    {
                        AccountId = accountId,
                        Reason = "Education account not found"
                    });
                    response.FailedCount++;
                    continue;
                }

                if (!educationAccount.IsActive)
                {
                    response.FailedOperations.Add(new FailedAccountOperation
                    {
                        AccountId = accountId,
                        Reason = "Account is already inactive"
                    });
                    response.FailedCount++;
                    continue;
                }

                educationAccount.IsActive = false;
                educationAccount.ClosedDate = DateTime.UtcNow;
                educationAccount.UpdatedAt = DateTime.UtcNow;
                
                await educationAccountRepo.UpdateAsync(educationAccount);
                response.SuccessfulIds.Add(accountId);
                response.SuccessCount++;
            }
            catch (Exception ex)
            {
                response.FailedOperations.Add(new FailedAccountOperation
                {
                    AccountId = accountId,
                    Reason = $"Error: {ex.Message}"
                });
                response.FailedCount++;
            }
        }

        await _unitOfWork.SaveAsync();
        return response;
    }

    public async Task UpdateAccountHolderAsync(EditAccountHolderRequest request)
    {
        var accountHolderRepo = _unitOfWork.GetRepository<AccountHolder>();

        var existingAccountHolder = accountHolderRepo.Entities
            .FirstOrDefault(ah => ah.Id == request.AccountHolderId);

        if (existingAccountHolder == null)
        {
            throw new NotFoundException("ACCOUNT_HOLDER_NOT_FOUND", $"Account holder with ID {request.AccountHolderId} not found.");
        }

        existingAccountHolder.Email = request.Email;
        existingAccountHolder.ContactNumber = request.PhoneNumber;
        existingAccountHolder.RegisteredAddress = request.RegisteredAddress;
        existingAccountHolder.MailingAddress = request.MailingAddress;
        existingAccountHolder.UpdatedAt = DateTime.UtcNow;

        await accountHolderRepo.UpdateAsync(existingAccountHolder);
        await _unitOfWork.SaveAsync();
    }
}
