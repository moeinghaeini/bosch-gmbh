using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace IndustrialAutomation.API.Middleware;

public class InputValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<InputValidationMiddleware> _logger;

    public InputValidationMiddleware(RequestDelegate next, ILogger<InputValidationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Method is "POST" or "PUT" or "PATCH")
        {
            context.Request.EnableBuffering();
            var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
            context.Request.Body.Position = 0;

            if (!string.IsNullOrEmpty(body))
            {
                // Check for potential SQL injection patterns
                if (ContainsSqlInjectionPatterns(body))
                {
                    _logger.LogWarning("Potential SQL injection attempt from {IpAddress}", 
                        context.Connection.RemoteIpAddress?.ToString());
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync("Invalid input detected");
                    return;
                }

                // Check for potential XSS patterns
                if (ContainsXssPatterns(body))
                {
                    _logger.LogWarning("Potential XSS attempt from {IpAddress}", 
                        context.Connection.RemoteIpAddress?.ToString());
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync("Invalid input detected");
                    return;
                }

                // Validate JSON structure
                if (context.Request.ContentType?.Contains("application/json") == true)
                {
                    try
                    {
                        JsonDocument.Parse(body);
                    }
                    catch (JsonException)
                    {
                        _logger.LogWarning("Invalid JSON in request body from {IpAddress}", 
                            context.Connection.RemoteIpAddress?.ToString());
                        context.Response.StatusCode = 400;
                        await context.Response.WriteAsync("Invalid JSON format");
                        return;
                    }
                }
            }
        }

        await _next(context);
    }

    private static bool ContainsSqlInjectionPatterns(string input)
    {
        var sqlPatterns = new[]
        {
            @"(\b(SELECT|INSERT|UPDATE|DELETE|DROP|CREATE|ALTER|EXEC|EXECUTE)\b)",
            @"(\b(UNION|OR|AND)\b.*\b(SELECT|INSERT|UPDATE|DELETE)\b)",
            @"(\b(SCRIPT|JAVASCRIPT|VBSCRIPT)\b)",
            @"(--|#|\/\*|\*\/)",
            @"(\b(WAITFOR|DELAY)\b)",
            @"(\b(CHAR|ASCII|SUBSTRING)\b)",
            @"(\b(CAST|CONVERT)\b)"
        };

        return sqlPatterns.Any(pattern => Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase));
    }

    private static bool ContainsXssPatterns(string input)
    {
        var xssPatterns = new[]
        {
            @"<script\b[^<]*(?:(?!<\/script>)<[^<]*)*<\/script>",
            @"javascript:",
            @"on\w+\s*=",
            @"<iframe\b[^>]*>",
            @"<object\b[^>]*>",
            @"<embed\b[^>]*>",
            @"<link\b[^>]*>",
            @"<meta\b[^>]*>",
            @"<style\b[^>]*>",
            @"expression\s*\(",
            @"url\s*\(",
            @"@import"
        };

        return xssPatterns.Any(pattern => Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase));
    }
}

public static class InputValidationMiddlewareExtensions
{
    public static IApplicationBuilder UseInputValidation(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<InputValidationMiddleware>();
    }
}
