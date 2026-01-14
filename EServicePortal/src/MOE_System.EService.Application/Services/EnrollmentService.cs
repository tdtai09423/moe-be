using MOE_System.EService.Application.DTOs;
using MOE_System.EService.Application.Interfaces.Services;
using MOE_System.EService.Application.Interfaces;
using MOE_System.EService.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using MOE_System.EService.Application.Common;
using System.Reflection.Metadata.Ecma335;
using Azure;
using Microsoft.IdentityModel.Tokens;
using MOE_System.Domain.Common;
using MOE_System.EService.Domain.Entities;
using static MOE_System.EService.Domain.Common.BaseException;

namespace MOE_System.EService.Application.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public EnrollmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PaginatedList<ActiveCoursesResponse>> GetActiveCoursesAsync(string accountHolderId, int pageIndex, int pageSize)
        {
            if (string.IsNullOrWhiteSpace(accountHolderId))
            {
                throw new BaseException.BadRequestException("ID must not be empty or null!");
            }

            var repo = _unitOfWork.GetRepository<Enrollment>();

            var query = repo.Entities.AsNoTracking()
                .Where(x => x.EducationAccount.AccountHolder.Id == accountHolderId
                && x.Status == "Active")
                .Include(x => x.Course).ThenInclude(x => x.Provider);

            var pagedEntities = await repo.GetPagging(query, pageIndex, pageSize);

            var dtoItems = pagedEntities.Items.Select(
                e => new ActiveCoursesResponse
                {
                    EnrollmentId = e.Id,
                    CourseName = e.Course?.CourseName ?? "",
                    CourseCode = e.Course?.CourseCode ?? "",
                    ProviderName = e.Course?.Provider?.Name ?? "",
                    EnrollDate = e.EnrollDate,
                    Status = e.Status,
                }
                ).ToList();

            var result = new PaginatedList<ActiveCoursesResponse>(
                items: dtoItems,
                count: pagedEntities.TotalCount,
                pageIndex: pageIndex,
                pageSize: pageSize
                );

            return result;
        }

    }
}
