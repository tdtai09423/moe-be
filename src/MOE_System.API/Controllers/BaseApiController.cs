using Microsoft.AspNetCore.Mvc;
using MOE_System.Application.Common;

namespace MOE_System.API.Controllers;

[ApiController]
public abstract class BaseApiController : ControllerBase
{
    protected ActionResult<ApiResponse<T>> Success<T>(T data, string message = "Success")
    {
        return Ok(ApiResponse<T>.SuccessResponse(data, message));
    }

    protected ActionResult<ApiResponse> Success(string message = "Success")
    {
        return Ok(ApiResponse.SuccessResponse(message));
    }

    protected ActionResult<ApiResponse<T>> Created<T>(T data, string message = "Created successfully")
    {
        return StatusCode(201, ApiResponse<T>.SuccessResponse(data, message));
    }

    protected ActionResult<ApiResponse<PaginatedList<T>>> Paginated<T>(
        List<T> items, 
        int pageNumber, 
        int pageSize, 
        int totalCount,
        string message = "Data retrieved successfully")
    {
        var paginatedData = new PaginatedList<T>(items, totalCount, pageNumber, pageSize);
        return Ok(ApiResponse<PaginatedList<T>>.SuccessResponse(paginatedData, message));
    }
}
