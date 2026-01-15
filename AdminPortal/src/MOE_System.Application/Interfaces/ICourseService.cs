using MOE_System.Application.Common;
using MOE_System.Application.DTOs.Course.Request;
using MOE_System.Application.DTOs.Course.Response;

namespace MOE_System.Application.Interfaces
{
    public interface ICourseService
    {
        Task<PaginatedList<CourseListResponse>> GetCoursesAsync(GetCourseRequest request, CancellationToken cancellationToken);
        Task<CourseDetailResponse?> GetCourseDetailAsync(string courseCode, CancellationToken cancellationToken = default);
        Task<NonEnrolledAccountResponse> GetNonEnrolledAccountAsync(string courseId, CancellationToken cancellationToken = default);
        Task<CourseResponse> AddCourseAsync(AddCourseRequest request);
        Task BulkEnrollAccountAsync(BulkEnrollAccountAsync request);
        Task BulkRemoveEnrolledAccountAsync(BulkRemoveEnrolledAccountRequest request);
    }
}
