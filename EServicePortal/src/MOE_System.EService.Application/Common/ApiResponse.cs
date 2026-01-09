namespace MOE_System.EService.Application.Common;

public class ApiResponse<T>
{
    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;

    public T? Data { get; set; }

    public object? Error { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public static ApiResponse<T> SuccessResponse(T data, string message = "Success")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data,
            Error = null,
            Timestamp = DateTime.UtcNow
        };
    }

    public static ApiResponse<T> SuccessResponse(string message = "Success")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = default,
            Error = null,
            Timestamp = DateTime.UtcNow
        };
    }

    public static ApiResponse<T> ErrorResponse(string message, object? error = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Data = default,
            Error = error,
            Timestamp = DateTime.UtcNow
        };
    }

    public static ApiResponse<T> ErrorResponse(Exception exception)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = exception.Message,
            Data = default,
            Error = new
            {
                Type = exception.GetType().Name,
                Message = exception.Message,
                StackTrace = exception.StackTrace
            },
            Timestamp = DateTime.UtcNow
        };
    }
}


public class ApiResponse : ApiResponse<object>
{

    public static new ApiResponse SuccessResponse(string message = "Success")
    {
        return new ApiResponse
        {
            Success = true,
            Message = message,
            Data = null,
            Error = null,
            Timestamp = DateTime.UtcNow
        };
    }

    public static new ApiResponse ErrorResponse(string message, object? error = null)
    {
        return new ApiResponse
        {
            Success = false,
            Message = message,
            Data = null,
            Error = error,
            Timestamp = DateTime.UtcNow
        };
    }
}
