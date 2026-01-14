using Microsoft.AspNetCore.Mvc;
using MOE_System.Application.Common;
using MOE_System.Application.DTOs.Course.Request;
using MOE_System.Application.DTOs.Course.Response;
using MOE_System.Application.Interfaces;

namespace MOE_System.API.Controllers
{
    [Route("api/v1/admin/courses")]
    public class CourseController : BaseApiController
    {
        private readonly ICourseService _courseService;

        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        /// <summary>
        /// Add a new course to the system
        /// </summary>
        /// <param name="request">The course details to add</param>
        /// <returns>The created course information</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<CourseResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<CourseResponse>>> AddCourse([FromBody] AddCourseRequest request)
        {
            var course = await _courseService.AddCourseAsync(request);
            return Created(course, "Course added successfully");
        }
    }
}
