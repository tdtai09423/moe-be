using MOE_System.Application.DTOs.Course.Request;
using MOE_System.Application.DTOs.Course.Response;

namespace MOE_System.Application.Interfaces
{
    public interface ICourseService
    {
        Task<CourseResponse> AddCourseAsync(AddCourseRequest request);
    }
}
