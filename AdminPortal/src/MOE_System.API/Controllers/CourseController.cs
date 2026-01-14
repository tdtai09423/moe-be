using Microsoft.AspNetCore.Mvc;
using MOE_System.Application.DTOs.Course.Request;
using MOE_System.Application.Interfaces.Services;

namespace MOE_System.API.Controllers;

[ApiController]
[Route("api/v1/admin/courses")]
public class CourseController : ControllerBase
{
    private readonly ICourseService _courseService;

    public CourseController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCourses([FromQuery] GetCourseRequest request, CancellationToken cancellationToken)
    {
        var courses = await _courseService.GetCoursesAsync(request, cancellationToken);
        return Ok(courses);
    }
}