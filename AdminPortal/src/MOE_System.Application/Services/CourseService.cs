using System.Linq.Expressions;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using MOE_System.Application.Common;
using MOE_System.Application.Common.Course;
using MOE_System.Application.Common.Interfaces;
using MOE_System.Application.DTOs.Course.Request;
using MOE_System.Application.DTOs.Course.Response;
using MOE_System.Application.Interfaces;
using MOE_System.Domain.Entities;
using static MOE_System.Domain.Common.BaseException;

namespace MOE_System.Application.Services
{
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

        public async Task<CourseResponse> AddCourseAsync(AddCourseRequest request)
        {
            // Validate that the provider exists
            var providerRepo = _unitOfWork.GetRepository<Provider>();
            var provider = await providerRepo.Entities
                .FirstOrDefaultAsync(p => p.Id == request.ProviderId);

            if (provider == null)
            {
                throw new NotFoundException("PROVIDER_NOT_FOUND", $"Provider with ID {request.ProviderId} not found.");
            }

            // Calculate duration in months
            var durationMonths = CalculateDurationInMonths(request.CourseStartDate, request.CourseEndDate);

            // Calculate total fee and fee per cycle based on payment option
            decimal totalFee = 0;
            decimal? feePerCycle = null;

            if (request.PaymentOption == "One-time")
            {
                totalFee = request.TotalFee ?? 0;
            }
            else if (request.PaymentOption == "Recurring")
            {
                if (request.FeePerCycle.HasValue)
                {
                    feePerCycle = request.FeePerCycle.Value;
                    // Calculate total fee based on fee per cycle and billing cycle
                    totalFee = CalculateTotalFeeFromCycle(feePerCycle.Value, request.BillingCycle!, durationMonths);
                }
                else if (request.TotalFee.HasValue)
                {
                    totalFee = request.TotalFee.Value;
                    // Calculate fee per cycle based on total fee and billing cycle
                    feePerCycle = CalculateFeePerCycle(totalFee, request.BillingCycle!, durationMonths);
                }
            }

            // Generate course code if not provided
            var courseCode = !string.IsNullOrWhiteSpace(request.CourseCode) 
                ? request.CourseCode 
                : await GenerateCourseCodeAsync(request.CourseName);

            // Create the course entity
            var course = new Course
            {
                CourseName = request.CourseName,
                CourseCode = courseCode,
                ProviderId = request.ProviderId,
                LearningType = request.ModeOfTraining,
                StartDate = request.CourseStartDate,
                EndDate = request.CourseEndDate,
                PaymentType = request.PaymentOption,
                FeeAmount = totalFee,
                FeePerCycle = feePerCycle,
                BillingCycle = request.BillingCycle,
                DurationByMonth = durationMonths,
                TermName = request.TermName ?? string.Empty,
                Status = "Active",
                CreatedAt = DateTime.UtcNow
            };

            // Add course to repository
            var courseRepo = _unitOfWork.GetRepository<Course>();
            await courseRepo.InsertAsync(course);
            await _unitOfWork.SaveAsync();

            // Return the response
            return new CourseResponse
            {
                Id = course.Id,
                CourseName = course.CourseName,
                CourseCode = course.CourseCode,
                ProviderId = course.ProviderId,
                ProviderName = provider.Name,
                ModeOfTraining = course.LearningType,
                CourseStartDate = course.StartDate,
                CourseEndDate = course.EndDate,
                PaymentOption = course.PaymentType,
                TotalFee = course.FeeAmount,
                BillingCycle = course.BillingCycle,
                FeePerCycle = course.FeePerCycle,
                TermName = course.TermName,
                Status = course.Status,
                CreatedAt = course.CreatedAt
            };
        }

        private int CalculateDurationInMonths(DateTime startDate, DateTime endDate)
        {
            int months = ((endDate.Year - startDate.Year) * 12) + endDate.Month - startDate.Month;
            
            // Add 1 if the end day is greater than or equal to start day
            if (endDate.Day >= startDate.Day)
            {
                months++;
            }

            return months > 0 ? months : 1;
        }

        private decimal CalculateTotalFeeFromCycle(decimal feePerCycle, string billingCycle, int durationMonths)
        {
            int numberOfCycles = billingCycle switch
            {
                "Monthly" => durationMonths,
                "Quarterly" => (int)Math.Ceiling(durationMonths / 3.0),
                "Biannually" => (int)Math.Ceiling(durationMonths / 6.0),
                "Yearly" => (int)Math.Ceiling(durationMonths / 12.0),
                _ => durationMonths
            };

            return feePerCycle * numberOfCycles;
        }

        private decimal CalculateFeePerCycle(decimal totalFee, string billingCycle, int durationMonths)
        {
            int numberOfCycles = billingCycle switch
            {
                "Monthly" => durationMonths,
                "Quarterly" => (int)Math.Ceiling(durationMonths / 3.0),
                "Biannually" => (int)Math.Ceiling(durationMonths / 6.0),
                "Yearly" => (int)Math.Ceiling(durationMonths / 12.0),
                _ => durationMonths
            };

            return numberOfCycles > 0 ? totalFee / numberOfCycles : totalFee;
        }

        private async Task<string> GenerateCourseCodeAsync(string courseName)
        {
            // Generate a simple course code from course name
            var prefix = string.Concat(courseName.Split(' ')
                .Where(w => !string.IsNullOrWhiteSpace(w))
                .Take(3)
                .Select(w => char.ToUpper(w[0])));

            // Ensure we have at least a default prefix
            if (string.IsNullOrEmpty(prefix))
            {
                prefix = "CRS";
            }

            // Get count of existing courses with similar prefix to generate unique code
            var courseRepo = _unitOfWork.GetRepository<Course>();
            var existingCount = await courseRepo.Entities
                .Where(c => c.CourseCode.StartsWith(prefix))
                .CountAsync();

            return $"{prefix}{(existingCount + 1):D4}";
        }
    }
}
