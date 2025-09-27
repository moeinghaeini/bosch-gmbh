using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

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
        try
        {
            // Add security headers
            AddSecurityHeaders(context);
            
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in SecurityHeadersMiddleware");
            throw;
        }
    }

    private void AddSecurityHeaders(HttpContext context)
    {
        var response = context.Response;
        
        // Prevent clickjacking
        response.Headers.Add("X-Frame-Options", "DENY");
        
        // Prevent MIME type sniffing
        response.Headers.Add("X-Content-Type-Options", "nosniff");
        
        // XSS Protection
        response.Headers.Add("X-XSS-Protection", "1; mode=block");
        
        // Referrer Policy
        response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
        
        // Content Security Policy
        response.Headers.Add("Content-Security-Policy", 
            "default-src 'self'; " +
            "script-src 'self' 'unsafe-inline' 'unsafe-eval' https://cdn.jsdelivr.net; " +
            "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com; " +
            "font-src 'self' https://fonts.gstatic.com; " +
            "img-src 'self' data: https:; " +
            "connect-src 'self' https:; " +
            "frame-ancestors 'none'; " +
            "base-uri 'self'; " +
            "form-action 'self'");
        
        // Permissions Policy
        response.Headers.Add("Permissions-Policy", 
            "geolocation=(), " +
            "microphone=(), " +
            "camera=(), " +
            "payment=(), " +
            "usb=(), " +
            "magnetometer=(), " +
            "gyroscope=(), " +
            "speaker=(), " +
            "vibrate=(), " +
            "fullscreen=(self), " +
            "sync-xhr=()");
        
        // Strict Transport Security
        if (context.Request.IsHttps)
        {
            response.Headers.Add("Strict-Transport-Security", 
                "max-age=31536000; includeSubDomains; preload");
        }
        
        // Cross-Origin Embedder Policy
        response.Headers.Add("Cross-Origin-Embedder-Policy", "require-corp");
        
        // Cross-Origin Opener Policy
        response.Headers.Add("Cross-Origin-Opener-Policy", "same-origin");
        
        // Cross-Origin Resource Policy
        response.Headers.Add("Cross-Origin-Resource-Policy", "same-origin");
        
        // Remove server header
        response.Headers.Remove("Server");
        
        // Add custom security headers
        response.Headers.Add("X-Permitted-Cross-Domain-Policies", "none");
        response.Headers.Add("X-Download-Options", "noopen");
        response.Headers.Add("X-DNS-Prefetch-Control", "off");
        
        // Cache control for sensitive endpoints
        if (context.Request.Path.StartsWithSegments("/api/auth"))
        {
            response.Headers.Add("Cache-Control", "no-store, no-cache, must-revalidate, proxy-revalidate");
            response.Headers.Add("Pragma", "no-cache");
            response.Headers.Add("Expires", "0");
        }
    }
}
