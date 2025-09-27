using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text;

namespace IndustrialAutomation.API.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestId = Guid.NewGuid().ToString();
        
        // Add request ID to context for tracing
        context.Items["RequestId"] = requestId;
        
        // Log request details
        await LogRequestAsync(context, requestId);
        
        // Capture response
        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            
            // Log response details
            await LogResponseAsync(context, requestId, stopwatch.ElapsedMilliseconds);
            
            // Restore response body
            await responseBody.CopyToAsync(originalBodyStream);
            context.Response.Body = originalBodyStream;
        }
    }

    private async Task LogRequestAsync(HttpContext context, string requestId)
    {
        var request = context.Request;
        
        var logData = new
        {
            RequestId = requestId,
            Method = request.Method,
            Path = request.Path.Value,
            QueryString = request.QueryString.Value,
            Headers = GetSafeHeaders(request.Headers),
            ContentType = request.ContentType,
            ContentLength = request.ContentLength,
            RemoteIpAddress = context.Connection.RemoteIpAddress?.ToString(),
            UserAgent = request.Headers.UserAgent.ToString(),
            Timestamp = DateTime.UtcNow
        };

        _logger.LogInformation("Request {RequestId}: {Method} {Path} from {RemoteIpAddress}", 
            requestId, request.Method, request.Path, context.Connection.RemoteIpAddress);

        // Log request body for non-GET requests (be careful with sensitive data)
        if (request.Method != "GET" && request.ContentLength > 0 && request.ContentLength < 1024 * 10) // 10KB limit
        {
            try
            {
                request.EnableBuffering();
                var buffer = new byte[Convert.ToInt32(request.ContentLength)];
                await request.Body.ReadAsync(buffer, 0, buffer.Length);
                var bodyAsText = Encoding.UTF8.GetString(buffer);
                request.Body.Position = 0;

                _logger.LogDebug("Request {RequestId} body: {Body}", requestId, bodyAsText);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to read request body for {RequestId}", requestId);
            }
        }
    }

    private async Task LogResponseAsync(HttpContext context, string requestId, long elapsedMilliseconds)
    {
        var response = context.Response;
        
        var logData = new
        {
            RequestId = requestId,
            StatusCode = response.StatusCode,
            ContentType = response.ContentType,
            ContentLength = response.Body.Length,
            ElapsedMilliseconds = elapsedMilliseconds,
            Timestamp = DateTime.UtcNow
        };

        var logLevel = response.StatusCode >= 500 ? LogLevel.Error :
                       response.StatusCode >= 400 ? LogLevel.Warning :
                       LogLevel.Information;

        _logger.Log(logLevel, "Response {RequestId}: {StatusCode} in {ElapsedMs}ms", 
            requestId, response.StatusCode, elapsedMilliseconds);

        // Log response body for errors (be careful with sensitive data)
        if (response.StatusCode >= 400 && response.Body.Length > 0 && response.Body.Length < 1024 * 5) // 5KB limit
        {
            try
            {
                response.Body.Seek(0, SeekOrigin.Begin);
                var responseBody = await new StreamReader(response.Body).ReadToEndAsync();
                response.Body.Seek(0, SeekOrigin.Begin);

                _logger.LogDebug("Response {RequestId} body: {Body}", requestId, responseBody);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to read response body for {RequestId}", requestId);
            }
        }
    }

    private Dictionary<string, string> GetSafeHeaders(IHeaderDictionary headers)
    {
        var safeHeaders = new Dictionary<string, string>();
        var sensitiveHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Authorization",
            "Cookie",
            "X-API-Key",
            "X-Auth-Token",
            "X-Forwarded-For",
            "X-Real-IP"
        };

        foreach (var header in headers)
        {
            if (!sensitiveHeaders.Contains(header.Key))
            {
                safeHeaders[header.Key] = header.Value.ToString();
            }
            else
            {
                safeHeaders[header.Key] = "[REDACTED]";
            }
        }

        return safeHeaders;
    }
}
