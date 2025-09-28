namespace IndustrialAutomation.API.Middleware;

public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SecurityHeadersMiddleware> _logger;

    public SecurityHeadersMiddleware(RequestDelegate next, ILogger<SecurityHeadersMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Security headers
        context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Add("X-Frame-Options", "DENY");
        context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
        context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
        context.Response.Headers.Add("Permissions-Policy", "geolocation=(), microphone=(), camera=(), payment=(), usb=(), magnetometer=(), gyroscope=(), speaker=(), vibrate=(), fullscreen=(self), sync-xhr=()");
        
        // Content Security Policy
        var csp = "default-src 'self'; " +
                 "script-src 'self' 'unsafe-inline' 'unsafe-eval' https://cdn.jsdelivr.net; " +
                 "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com; " +
                 "font-src 'self' https://fonts.gstatic.com; " +
                 "img-src 'self' data: https:; " +
                 "connect-src 'self' https:; " +
                 "frame-ancestors 'none'; " +
                 "base-uri 'self'; " +
                 "form-action 'self'";
        context.Response.Headers.Add("Content-Security-Policy", csp);
        
        // Strict Transport Security (HTTPS only)
        if (context.Request.IsHttps)
        {
            context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains; preload");
        }
        
        // Remove server header
        context.Response.Headers.Remove("Server");
        
        // Add custom security headers
        context.Response.Headers.Add("X-Permitted-Cross-Domain-Policies", "none");
        context.Response.Headers.Add("X-Download-Options", "noopen");
        context.Response.Headers.Add("X-DNS-Prefetch-Control", "off");

        await _next(context);
    }
}

public static class SecurityHeadersMiddlewareExtensions
{
    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<SecurityHeadersMiddleware>();
    }
}