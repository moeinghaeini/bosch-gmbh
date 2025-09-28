using System.Diagnostics;
using System.Text.Json;
using IndustrialAutomation.Infrastructure.Data;
using IndustrialAutomation.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace IndustrialAutomation.API.Middleware;

public class AuditLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuditLoggingMiddleware> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public AuditLoggingMiddleware(RequestDelegate next, ILogger<AuditLoggingMiddleware> logger, IServiceScopeFactory serviceScopeFactory)
    {
        _next = next;
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestId = context.TraceIdentifier;
        var userId = context.Items["UserId"]?.ToString();
        var userRole = context.Items["UserRole"]?.ToString();
        
        var auditEntry = new AuditEntry
        {
            RequestId = requestId,
            UserId = userId,
            UserRole = userRole,
            Method = context.Request.Method,
            Path = context.Request.Path.Value,
            QueryString = context.Request.QueryString.Value,
            UserAgent = context.Request.Headers["User-Agent"].FirstOrDefault(),
            IpAddress = context.Connection.RemoteIpAddress?.ToString(),
            Timestamp = DateTime.UtcNow
        };

        // Capture request body for POST/PUT/PATCH requests
        if (context.Request.Method is "POST" or "PUT" or "PATCH")
        {
            context.Request.EnableBuffering();
            var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
            context.Request.Body.Position = 0;
            auditEntry.RequestBody = body;
        }

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            auditEntry.Exception = ex.Message;
            auditEntry.StatusCode = 500;
            throw;
        }
        finally
        {
            stopwatch.Stop();
            auditEntry.StatusCode = context.Response.StatusCode;
            auditEntry.Duration = stopwatch.ElapsedMilliseconds;
            auditEntry.ResponseSize = context.Response.ContentLength ?? 0;

            // Log to database asynchronously
            _ = Task.Run(async () => await LogAuditEntryAsync(auditEntry));
        }
    }

    private async Task LogAuditEntryAsync(AuditEntry auditEntry)
    {
        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IndustrialAutomationDbContext>();
            
            var auditLog = new AuditLog
            {
                RequestId = auditEntry.RequestId,
                UserId = auditEntry.UserId,
                UserRole = auditEntry.UserRole,
                Method = auditEntry.Method,
                Path = auditEntry.Path,
                QueryString = auditEntry.QueryString,
                RequestBody = auditEntry.RequestBody,
                StatusCode = auditEntry.StatusCode,
                Duration = auditEntry.Duration,
                ResponseSize = auditEntry.ResponseSize,
                UserAgent = auditEntry.UserAgent,
                IpAddress = auditEntry.IpAddress,
                Exception = auditEntry.Exception,
                Timestamp = auditEntry.Timestamp
            };

            context.AuditLogs.Add(auditLog);
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log audit entry for request {RequestId}", auditEntry.RequestId);
        }
    }

    private class AuditEntry
    {
        public string RequestId { get; set; } = string.Empty;
        public string? UserId { get; set; }
        public string? UserRole { get; set; }
        public string Method { get; set; } = string.Empty;
        public string? Path { get; set; }
        public string? QueryString { get; set; }
        public string? RequestBody { get; set; }
        public int StatusCode { get; set; }
        public long Duration { get; set; }
        public long ResponseSize { get; set; }
        public string? UserAgent { get; set; }
        public string? IpAddress { get; set; }
        public string? Exception { get; set; }
        public DateTime Timestamp { get; set; }
    }
}

public static class AuditLoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseAuditLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<AuditLoggingMiddleware>();
    }
}
