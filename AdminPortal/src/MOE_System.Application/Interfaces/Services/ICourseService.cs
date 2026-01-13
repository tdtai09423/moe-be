using MOE_System.Application.Common;
using MOE_System.Application.DTOs.Course.Request;
using MOE_System.Application.DTOs.Course.Response;

namespace MOE_System.Application.Interfaces.Services;

public interface ICourseService
{
    Task<PaginatedList<CourseListResponse>> GetCoursesAsync(GetCourseRequest request, CancellationToken cancellationToken = default);
}   