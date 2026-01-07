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

    protected ActionResult<ApiResponse<T>> BadRequestError<T>(string message, object? error = null)
    {
        return BadRequest(ApiResponse<T>.ErrorResponse(message, error));
    }

    protected ActionResult<ApiResponse> BadRequestError(string message, object? error = null)
    {
        return BadRequest(ApiResponse.ErrorResponse(message, error));
    }

    protected ActionResult<ApiResponse<T>> NotFoundError<T>(string message = "Resource not found")
    {
        return NotFound(ApiResponse<T>.ErrorResponse(message));
    }

    protected ActionResult<ApiResponse> NotFoundError(string message = "Resource not found")
    {
        return NotFound(ApiResponse.ErrorResponse(message));
    }

    protected ActionResult<ApiResponse<T>> UnauthorizedError<T>(string message = "Unauthorized")
    {
        return Unauthorized(ApiResponse<T>.ErrorResponse(message));
    }

    protected ActionResult<ApiResponse> UnauthorizedError(string message = "Unauthorized")
    {
        return Unauthorized(ApiResponse.ErrorResponse(message));
    }

    protected ActionResult<ApiResponse<T>> ForbiddenError<T>(string message = "Forbidden")
    {
        return StatusCode(403, ApiResponse<T>.ErrorResponse(message));
    }

    protected ActionResult<ApiResponse> ForbiddenError(string message = "Forbidden")
    {
        return StatusCode(403, ApiResponse.ErrorResponse(message));
    }

    protected ActionResult<ApiResponse<T>> InternalServerError<T>(string message = "Internal server error", object? error = null)
    {
        return StatusCode(500, ApiResponse<T>.ErrorResponse(message, error));
    }

    protected ActionResult<ApiResponse> InternalServerError(string message = "Internal server error", object? error = null)
    {
        return StatusCode(500, ApiResponse.ErrorResponse(message, error));
    }

    protected ActionResult<ApiResponse<T>> ValidationError<T>(Dictionary<string, List<string>> fieldErrors)
    {
        var error = new ErrorDetails
        {
            Code = "VALIDATION_ERROR",
            Message = "One or more validation errors occurred",
            FieldErrors = fieldErrors
        };

        return BadRequest(ApiResponse<T>.ErrorResponse("Validation failed", error));
    }

    protected ActionResult<ApiResponse> ValidationError(Dictionary<string, List<string>> fieldErrors)
    {
        var error = new ErrorDetails
        {
            Code = "VALIDATION_ERROR",
            Message = "One or more validation errors occurred",
            FieldErrors = fieldErrors
        };

        return BadRequest(ApiResponse.ErrorResponse("Validation failed", error));
    }

    protected ActionResult<ApiResponse<PaginatedResponse<T>>> Paginated<T>(
        List<T> items, 
        int pageNumber, 
        int pageSize, 
        int totalCount,
        string message = "Data retrieved successfully")
    {
        var paginatedData = PaginatedResponse<T>.Create(items, pageNumber, pageSize, totalCount);
        return Ok(ApiResponse<PaginatedResponse<T>>.SuccessResponse(paginatedData, message));
    }
}
