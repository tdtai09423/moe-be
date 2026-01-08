using Microsoft.EntityFrameworkCore;
using MOE_System.Application.DTOs;
using MOE_System.Application.Interfaces;
using MOE_System.Domain.Entities;

namespace MOE_System.Application.Services;

public class AccountHolderService : IAccountHolderService
{
    private readonly IUnitOfWork _unitOfWork;

    public AccountHolderService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<AccountHolderDto?> GetAccountHolderByIdAsync(string accountHolderId)
    {
        var accountHolder = await _unitOfWork.GetRepository<AccountHolder>()
            .Entities
            .Include(ah => ah.EducationAccount)
            .Where(ah => ah.Id == accountHolderId && ah.DeletedAt == null)
            .Select(ah => new AccountHolderDto
            {
                Id = ah.Id,
                FirstName = ah.FirstName,
                LastName = ah.LastName,
                DateOfBirth = ah.DateOfBirth,
                Address = ah.Address,
                Email = ah.Email,
                ContactNumber = ah.ContactNumber,
                NRIC = ah.NRIC,
                CitizenId = ah.CitizenId,
                Gender = ah.Gender,
                ContLearningStatus = ah.ContLearningStatus,
                EducationLevel = ah.EducationLevel,
                SchoolingStatus = ah.SchoolingStatus,
                EducationAccount = ah.EducationAccount != null ? new EducationAccountInfoDto
                {
                    Id = ah.EducationAccount.Id,
                    UserName = ah.EducationAccount.UserName,
                    Balance = ah.EducationAccount.Balance,
                    IsActive = ah.EducationAccount.IsActive,
                    LastLoginAt = ah.EducationAccount.LastLoginAt
                } : null
            })
            .FirstOrDefaultAsync();

        return accountHolder;
    }

    public async Task<List<ActiveCourseDto>> GetActiveCoursesAsync(string accountHolderId)
    {
        var accountHolder = await _unitOfWork.GetRepository<AccountHolder>()
            .Entities
            .Include(ah => ah.EducationAccount)
            .Where(ah => ah.Id == accountHolderId && ah.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (accountHolder?.EducationAccount == null)
        {
            return new List<ActiveCourseDto>();
        }

        var activeCourses = await _unitOfWork.GetRepository<Enrollment>()
            .Entities
            .Include(e => e.CourseOffering)
                .ThenInclude(co => co!.Course)
                .ThenInclude(c => c!.Provider)
            .Where(e => e.EducationAccountId == accountHolder.EducationAccount.Id 
                        && e.Status == "Active")
            .Select(e => new ActiveCourseDto
            {
                CourseId = e.CourseOffering!.Course!.Id,
                CourseName = e.CourseOffering!.Course!.CourseName,
                CourseCode = e.CourseOffering!.Course!.CourseCode,
                ProviderName = e.CourseOffering!.Course!.Provider!.Name,
                TermName = e.CourseOffering!.TermName,
                StartDate = e.CourseOffering!.StartDate,
                EndDate = e.CourseOffering!.EndDate,
                EnrollDate = e.EnrollDate,
                EnrollmentStatus = e.Status
            })
            .ToListAsync();

        return activeCourses;
    }
}
