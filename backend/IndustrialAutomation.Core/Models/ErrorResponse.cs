namespace IndustrialAutomation.Core.Models;

public class ErrorResponse
{
    public string Error { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Details { get; set; }
    public string? TraceId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? Path { get; set; }
    public Dictionary<string, object>? Extensions { get; set; }
}

public class ValidationErrorResponse : ErrorResponse
{
    public List<ValidationError> Errors { get; set; } = new();
}

public class ValidationError
{
    public string Field { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public object? AttemptedValue { get; set; }
}

public class BusinessException : Exception
{
    public string ErrorCode { get; set; } = string.Empty;
    public Dictionary<string, object>? Extensions { get; set; }

    public BusinessException(string message, string errorCode = "BUSINESS_ERROR") : base(message)
    {
        ErrorCode = errorCode;
    }

    public BusinessException(string message, Exception innerException, string errorCode = "BUSINESS_ERROR") : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}

public class ValidationException : BusinessException
{
    public List<ValidationError> ValidationErrors { get; set; } = new();

    public ValidationException(string message, List<ValidationError> errors) : base(message, "VALIDATION_ERROR")
    {
        ValidationErrors = errors;
    }
}

public class NotFoundException : BusinessException
{
    public NotFoundException(string resource, object id) : base($"{resource} with ID {id} not found", "NOT_FOUND")
    {
    }
}

public class UnauthorizedException : BusinessException
{
    public UnauthorizedException(string message = "Unauthorized access") : base(message, "UNAUTHORIZED")
    {
    }
}

public class ForbiddenException : BusinessException
{
    public ForbiddenException(string message = "Access forbidden") : base(message, "FORBIDDEN")
    {
    }
}

public class ConflictException : BusinessException
{
    public ConflictException(string message) : base(message, "CONFLICT")
    {
    }
}

public class RateLimitException : BusinessException
{
    public RateLimitException(string message = "Rate limit exceeded") : base(message, "RATE_LIMIT")
    {
    }
}

public class ServiceUnavailableException : BusinessException
{
    public ServiceUnavailableException(string service, string message = "Service temporarily unavailable") : base($"{service}: {message}", "SERVICE_UNAVAILABLE")
    {
    }
}
