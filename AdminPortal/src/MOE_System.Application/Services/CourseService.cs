using Microsoft.EntityFrameworkCore;
using MOE_System.Application.Common;
using MOE_System.Application.Common.Interfaces;
using MOE_System.Application.DTOs.Course.Request;
using MOE_System.Application.DTOs.Course.Response;
using MOE_System.Application.Interfaces.Services;
using MOE_System.Domain.Entities;
using LinqKit;
using MOE_System.Application.Common.Course;
using System.Linq.Expressions;

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

        var predicate = BuildFilterPredicate(request);

        IQueryable<Course> query = courseRepo.Entities.AsNoTracking()
            .Where(predicate.Expand());

        query = ApplySorting(query, request.SortBy, request.SortDirection);

        var pagedCourses = await courseRepo.GetPagging(query, request.PageNumber, request.PageSize);

        var responses = pagedCourses.Items.Select(c => new CourseListResponse(
            c.CourseCode,
            c.CourseName,
            c.Provider != null ? c.Provider.Name : string.Empty,
            string.Empty,
            c.StartDate,
            c.EndDate,
            c.PaymentType,
            c.FeeAmount,
            c.Enrollments.Count
        )).ToList();

        return new PaginatedList<CourseListResponse>(responses, pagedCourses.TotalCount, pagedCourses.PageIndex, request.PageSize);
    }

    private static Expression<Func<Course, bool>> BuildFilterPredicate(GetCourseRequest request)
    {
        var predicate = PredicateBuilder.New<Course>(true);

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var keyword = request.SearchTerm.Trim();
            predicate = predicate.And(x => x.CourseName.Contains(keyword) || (x.Provider != null && x.Provider.Name.Contains(keyword)));
        }

        if (!string.IsNullOrWhiteSpace(request.Provider))
            predicate = predicate.And(x => x.Provider != null && x.Provider.Name == request.Provider);

        if (!string.IsNullOrWhiteSpace(request.PaymentType))
            predicate = predicate.And(x => x.PaymentType == request.PaymentType);

        if (!string.IsNullOrWhiteSpace(request.BillingCycle))
            predicate = predicate.And(x => x.BillingCycle == request.BillingCycle);

        if (request.StartDateFrom.HasValue)
            predicate = predicate.And(x => x.StartDate >= request.StartDateFrom.Value);

        if (request.StartDateTo.HasValue)
            predicate = predicate.And(x => x.StartDate <= request.StartDateTo.Value);

        if (request.EndDateFrom.HasValue)
            predicate = predicate.And(x => x.EndDate >= request.EndDateFrom.Value);

        if (request.EndDateTo.HasValue)
            predicate = predicate.And(x => x.EndDate <= request.EndDateTo.Value);

        if (request.TotalFeeMin.HasValue)
            predicate = predicate.And(x => x.FeeAmount >= request.TotalFeeMin.Value);

        if (request.TotalFeeMax.HasValue)
            predicate = predicate.And(x => x.FeeAmount <= request.TotalFeeMax.Value);

        return predicate;
    }

    private static IQueryable<Course> ApplySorting(IQueryable<Course> query, CourseSortField? sortBy, SortDirection? sortDirection)
    {
        return (sortBy, sortDirection) switch
        {
            (CourseSortField.CourseName, SortDirection.Asc) => query.OrderBy(c => c.CourseName),
            (CourseSortField.CourseName, SortDirection.Desc) => query.OrderByDescending(c => c.CourseName),
            (CourseSortField.Provider, SortDirection.Asc) => query.OrderBy(c => c.Provider!.Name),
            (CourseSortField.Provider, SortDirection.Desc) => query.OrderByDescending(c => c.Provider!.Name),
            (CourseSortField.TotalFee, SortDirection.Asc) => query.OrderBy(c => c.FeeAmount),
            (CourseSortField.TotalFee, SortDirection.Desc) => query.OrderByDescending(c => c.FeeAmount),
            (CourseSortField.CreatedAt, SortDirection.Asc) => query.OrderBy(c => c.CreatedAt),
            _ => query.OrderByDescending(c => c.CreatedAt),
        };
    }
}