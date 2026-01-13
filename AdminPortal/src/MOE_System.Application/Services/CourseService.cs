using Microsoft.EntityFrameworkCore;
using MOE_System.Application.Common;
using MOE_System.Application.Common.Interfaces;
using MOE_System.Application.DTOs.Course.Request;
using MOE_System.Application.DTOs.Course.Response;
using MOE_System.Application.Interfaces.Services;
using MOE_System.Domain.Common;
using MOE_System.Domain.Entities;

namespace MOE_System.Application.Services;

public class CourseService : ICourseService
{
    private readonly IUnitOfWork _unitOfWork;

    public CourseService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PaginatedList<CourseListResponse>> GetCoursesAsync(GetCourseRequest request, CancellationToken cancellationToken = default)
    {
        var courseRepo = _unitOfWork.GetRepository<Course>();

        var query = courseRepo.Entities.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.Trim().ToLower();
            query = query.Where(c => c.CourseName.ToLower().Contains(searchTerm));
        }
    
        query = query.OrderByDescending(c => c.CreatedAt);

        var pagedCourses = await courseRepo.GetPagging(query, request.PageNumber, request.PageSize);

        var reponseItems = pagedCourses.Items.Select(c => new CourseListResponse(
            c.CourseCode,
            c.CourseName,
            c.Provider!.Name,
            null!,
            c.StartDate,
            c.EndDate,
            c.PaymentType,
            c.FeeAmount,
            c.Enrollments!.Count
        )).ToList();

        return new PaginatedList<CourseListResponse>(reponseItems, pagedCourses.TotalCount, pagedCourses.PageIndex, pagedCourses.TotalPages);
    }

    public async Task<CourseDetailResponse?> GetCourseDetailAsync(string courseCode, CancellationToken cancellationToken = default)
    {
        var courseRepo = _unitOfWork.GetRepository<Course>();

        var course = await courseRepo.FirstOrDefaultAsync(
            predicate: c => c.CourseCode == courseCode,
            include: q => q
                .Include(c => c.Provider)
                .Include(c => c.Enrollments)
                    .ThenInclude(e => e.EducationAccount)
                .Include(c => c.Enrollments)
                    .ThenInclude(e => e.Invoices)
                        .ThenInclude(i => i.Transactions),
            cancellationToken: cancellationToken
        );

        if (course == null)
        {
            throw new BaseException.NotFoundException($"Course with code '{courseCode}' not found.");
        }

        return new CourseDetailResponse(
            course.CourseCode,
            course.CourseName,
            course.Provider?.Name ?? string.Empty,
            null!,
            course.Status,
            course.StartDate,
            course.EndDate, 
            course.PaymentType,
            course.PaymentType == "Recurring" ? course.BillingCycle : null,
            course.FeeAmount,
            course.Enrollments!.OrderByDescending(e => e.EnrollDate).Select(e =>
            {
                var totalPaid = e.Invoices?.SelectMany(i => i.Transactions ?? Enumerable.Empty<Transaction>()).Sum(t => t.Amount) ?? 0m;

                return new EnrolledStudent(
                    e.EducationAccount?.Id ?? string.Empty,
                    e.EducationAccount?.UserName ?? string.Empty,
                    totalPaid,
                    course.FeeAmount - totalPaid,
                    e.EnrollDate
                );
            }).ToList()
        );
    }
}