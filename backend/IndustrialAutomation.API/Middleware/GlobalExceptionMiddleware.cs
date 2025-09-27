using IndustrialAutomation.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace IndustrialAutomation.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var errorResponse = CreateErrorResponse(exception, context);
        response.StatusCode = GetStatusCode(exception);

        var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });

        await response.WriteAsync(jsonResponse);
    }

    private ErrorResponse CreateErrorResponse(Exception exception, HttpContext context)
    {
        var traceId = context.TraceIdentifier;
        var path = context.Request.Path;

        return exception switch
        {
            BusinessException businessEx => new ErrorResponse
            {
                Error = businessEx.ErrorCode,
                Message = businessEx.Message,
                Details = GetExceptionDetails(businessEx),
                TraceId = traceId,
                Path = path,
                Extensions = businessEx.Extensions
            },
            ValidationException validationEx => new ValidationErrorResponse
            {
                Error = validationEx.ErrorCode,
                Message = validationEx.Message,
                Details = GetExceptionDetails(validationEx),
                TraceId = traceId,
                Path = path,
                Extensions = validationEx.Extensions,
                Errors = validationEx.ValidationErrors
            },
            UnauthorizedAccessException => new ErrorResponse
            {
                Error = "UNAUTHORIZED",
                Message = "Unauthorized access",
                Details = GetExceptionDetails(exception),
                TraceId = traceId,
                Path = path
            },
            ArgumentException argEx => new ErrorResponse
            {
                Error = "INVALID_ARGUMENT",
                Message = argEx.Message,
                Details = GetExceptionDetails(argEx),
                TraceId = traceId,
                Path = path
            },
            TimeoutException => new ErrorResponse
            {
                Error = "TIMEOUT",
                Message = "Request timeout",
                Details = GetExceptionDetails(exception),
                TraceId = traceId,
                Path = path
            },
            HttpRequestException httpEx => new ErrorResponse
            {
                Error = "HTTP_ERROR",
                Message = "External service error",
                Details = GetExceptionDetails(httpEx),
                TraceId = traceId,
                Path = path
            },
            _ => new ErrorResponse
            {
                Error = "INTERNAL_ERROR",
                Message = "An internal error occurred",
                Details = GetExceptionDetails(exception),
                TraceId = traceId,
                Path = path
            }
        };
    }

    private int GetStatusCode(Exception exception)
    {
        return exception switch
        {
            NotFoundException => (int)HttpStatusCode.NotFound,
            UnauthorizedException => (int)HttpStatusCode.Unauthorized,
            ForbiddenException => (int)HttpStatusCode.Forbidden,
            ConflictException => (int)HttpStatusCode.Conflict,
            ValidationException => (int)HttpStatusCode.BadRequest,
            RateLimitException => (int)HttpStatusCode.TooManyRequests,
            ServiceUnavailableException => (int)HttpStatusCode.ServiceUnavailable,
            TimeoutException => (int)HttpStatusCode.RequestTimeout,
            HttpRequestException => (int)HttpStatusCode.BadGateway,
            _ => (int)HttpStatusCode.InternalServerError
        };
    }

    private string? GetExceptionDetails(Exception exception)
    {
        // In production, don't expose internal exception details
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
        {
            return null;
        }

        return exception.ToString();
    }
}
