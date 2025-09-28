using System.Collections.Concurrent;
using System.Net;

namespace IndustrialAutomation.API.Middleware;

public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitingMiddleware> _logger;
    private readonly ConcurrentDictionary<string, RateLimitInfo> _rateLimitStore = new();
    private readonly int _maxRequests;
    private readonly TimeSpan _timeWindow;

    public RateLimitingMiddleware(RequestDelegate next, ILogger<RateLimitingMiddleware> logger, IConfiguration configuration)
    {
        _next = next;
        _logger = logger;
        _maxRequests = configuration.GetValue<int>("RateLimit:MaxRequests", 100);
        _timeWindow = TimeSpan.FromMinutes(configuration.GetValue<int>("RateLimit:TimeWindowMinutes", 1));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var clientId = GetClientIdentifier(context);
        var now = DateTime.UtcNow;

        var rateLimitInfo = _rateLimitStore.AddOrUpdate(
            clientId,
            new RateLimitInfo { Count = 1, ResetTime = now.Add(_timeWindow) },
            (key, existing) =>
            {
                if (now > existing.ResetTime)
                {
                    return new RateLimitInfo { Count = 1, ResetTime = now.Add(_timeWindow) };
                }
                return new RateLimitInfo { Count = existing.Count + 1, ResetTime = existing.ResetTime };
            });

        // Add rate limit headers
        context.Response.Headers.Add("X-RateLimit-Limit", _maxRequests.ToString());
        context.Response.Headers.Add("X-RateLimit-Remaining", Math.Max(0, _maxRequests - rateLimitInfo.Count).ToString());
        context.Response.Headers.Add("X-RateLimit-Reset", new DateTimeOffset(rateLimitInfo.ResetTime).ToUnixTimeSeconds().ToString());

        if (rateLimitInfo.Count > _maxRequests)
        {
            _logger.LogWarning("Rate limit exceeded for client {ClientId}. Count: {Count}", clientId, rateLimitInfo.Count);
            
            context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
            context.Response.Headers.Add("Retry-After", ((int)_timeWindow.TotalSeconds).ToString());
            
            await context.Response.WriteAsync("Rate limit exceeded. Please try again later.");
            return;
        }

        await _next(context);
    }

    private string GetClientIdentifier(HttpContext context)
    {
        // Use IP address as primary identifier
        var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        
        // For authenticated users, use user ID for more granular rate limiting
        var userId = context.Items["UserId"]?.ToString();
        if (!string.IsNullOrEmpty(userId))
        {
            return $"user:{userId}";
        }
        
        return $"ip:{ipAddress}";
    }

    private class RateLimitInfo
    {
        public int Count { get; set; }
        public DateTime ResetTime { get; set; }
    }
}

public static class RateLimitingMiddlewareExtensions
{
    public static IApplicationBuilder UseRateLimiting(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RateLimitingMiddleware>();
    }
}