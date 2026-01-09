namespace MOE_System.Domain.Common;

/// <summary>
/// Base exception classes for the application
/// </summary>
public static class BaseException
{
    /// <summary>
    /// Core exception with custom status code and additional data
    /// </summary>
    public class CoreException : Exception
    {
        public int StatusCode { get; set; }
        public string Code { get; set; } = string.Empty;
        public object? AdditionalData { get; set; }

        public CoreException(string message, int statusCode = 500, string code = "CORE_ERROR") : base(message)
        {
            StatusCode = statusCode;
            Code = code;
        }
    }

    /// <summary>
    /// Bad request exception (400)
    /// </summary>
    public class BadRequestException : Exception
    {
        public int StatusCode => 400;
        public ErrorDetail? ErrorDetail { get; set; }

        public BadRequestException(string message) : base(message)
        {
            ErrorDetail = new ErrorDetail { ErrorCode = "BAD_REQUEST", ErrorMessage = message };
        }

        public BadRequestException(string code, string message) : base(message)
        {
            ErrorDetail = new ErrorDetail { ErrorCode = code, ErrorMessage = message };
        }
    }

    /// <summary>
    /// Not found exception (404)
    /// </summary>
    public class NotFoundException : Exception
    {
        public int StatusCode => 404;
        public ErrorDetail? ErrorDetail { get; set; }

        public NotFoundException(string message) : base(message)
        {
            ErrorDetail = new ErrorDetail { ErrorCode = "NOT_FOUND", ErrorMessage = message };
        }

        public NotFoundException(string code, string message) : base(message)
        {
            ErrorDetail = new ErrorDetail { ErrorCode = code, ErrorMessage = message };
        }
    }

    /// <summary>
    /// Unauthorized exception (401)
    /// </summary>
    public class UnauthorizedException : Exception
    {
        public int StatusCode => 401;
        public ErrorDetail? ErrorDetail { get; set; }

        public UnauthorizedException(string message) : base(message)
        {
            ErrorDetail = new ErrorDetail { ErrorCode = "UNAUTHORIZED", ErrorMessage = message };
        }

        public UnauthorizedException(string code, string message) : base(message)
        {
            ErrorDetail = new ErrorDetail { ErrorCode = code, ErrorMessage = message };
        }
    }

    /// <summary>
    /// Validation exception (422)
    /// </summary>
    public class ValidationException : Exception
    {
        public int StatusCode => 422;
        public ErrorDetail? ErrorDetail { get; set; }

        public ValidationException(string message) : base(message)
        {
            ErrorDetail = new ErrorDetail { ErrorCode = "VALIDATION_ERROR", ErrorMessage = message };
        }

        public ValidationException(string code, string message) : base(message)
        {
            ErrorDetail = new ErrorDetail { ErrorCode = code, ErrorMessage = message };
        }
    }

    /// <summary>
    /// Error detail structure
    /// </summary>
    public class ErrorDetail
    {
        public string ErrorCode { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
