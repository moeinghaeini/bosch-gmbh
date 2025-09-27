using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace IndustrialAutomation.API.Middleware;

public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitingMiddleware> _logger;
    private readonly IMemoryCache _cache;
    private readonly RateLimitOptions _options;

    public RateLimitingMiddleware(
        RequestDelegate next, 
        ILogger<RateLimitingMiddleware> logger,
        IMemoryCache cache,
        RateLimitOptions options)
    {
        _next = next;
        _logger = logger;
        _cache = cache;
        _options = options;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var clientId = GetClientIdentifier(context);
        var endpoint = GetEndpointIdentifier(context);

        if (await IsRateLimitedAsync(clientId, endpoint))
        {
            _logger.LogWarning("Rate limit exceeded for client {ClientId} on endpoint {Endpoint}", clientId, endpoint);
            
            context.Response.StatusCode = 429;
            context.Response.Headers.Add("Retry-After", _options.WindowSeconds.ToString());
            context.Response.Headers.Add("X-RateLimit-Limit", _options.MaxRequests.ToString());
            context.Response.Headers.Add("X-RateLimit-Remaining", "0");
            context.Response.Headers.Add("X-RateLimit-Reset", DateTimeOffset.UtcNow.AddSeconds(_options.WindowSeconds).ToUnixTimeSeconds().ToString());
            
            await context.Response.WriteAsync("Rate limit exceeded. Please try again later.");
            return;
        }

        await _next(context);
    }

    private string GetClientIdentifier(HttpContext context)
    {
        // Try to get user ID from JWT token first
        var userId = context.User?.FindFirst("user_id")?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            return $"user:{userId}";
        }

        // Fall back to IP address
        var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return $"ip:{ipAddress}";
    }

    private string GetEndpointIdentifier(HttpContext context)
    {
        var method = context.Request.Method;
        var path = context.Request.Path.Value?.ToLowerInvariant() ?? "";
        
        // Normalize path for rate limiting (remove IDs, etc.)
        var normalizedPath = NormalizePath(path);
        
        return $"{method}:{normalizedPath}";
    }

    private string NormalizePath(string path)
    {
        if (string.IsNullOrEmpty(path))
            return path;

        // Replace numeric IDs with placeholder
        var normalized = System.Text.RegularExpressions.Regex.Replace(path, @"/\d+", "/{id}");
        
        // Replace GUIDs with placeholder
        normalized = System.Text.RegularExpressions.Regex.Replace(normalized, @"/[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}", "/{guid}");
        
        return normalized;
    }

    private async Task<bool> IsRateLimitedAsync(string clientId, string endpoint)
    {
        var key = $"rate_limit:{clientId}:{endpoint}";
        var now = DateTime.UtcNow;
        var windowStart = now.AddSeconds(-_options.WindowSeconds);

        // Get existing requests in the current window
        if (!_cache.TryGetValue(key, out List<DateTime> requests))
        {
            requests = new List<DateTime>();
        }

        // Remove old requests outside the window
        requests.RemoveAll(t => t < windowStart);

        // Check if adding this request would exceed the limit
        if (requests.Count >= _options.MaxRequests)
        {
            return true;
        }

        // Add current request
        requests.Add(now);

        // Cache the updated list
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_options.WindowSeconds + 1)
        };
        
        _cache.Set(key, requests, cacheOptions);

        return false;
    }
}

public class RateLimitOptions
{
    public int MaxRequests { get; set; } = 100;
    public int WindowSeconds { get; set; } = 60;
    public Dictionary<string, RateLimitRule> EndpointRules { get; set; } = new();
}

public class RateLimitRule
{
    public int MaxRequests { get; set; }
    public int WindowSeconds { get; set; }
    public string[]? AllowedRoles { get; set; }
}
