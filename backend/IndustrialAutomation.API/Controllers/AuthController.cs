using IndustrialAutomation.Core.Interfaces;
using IndustrialAutomation.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;

namespace IndustrialAutomation.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Authenticate user and return JWT token
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _authService.LoginAsync(request);
            
            _logger.LogInformation("User {Email} logged in successfully", request.Email);
            
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Login failed for {Email}: {Message}", request.Email, ex.Message);
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for {Email}", request.Email);
            return StatusCode(500, new { message = "An error occurred during login" });
        }
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _authService.RegisterAsync(request);
            
            _logger.LogInformation("User {Email} registered successfully", request.Email);
            
            return CreatedAtAction(nameof(GetUserInfo), new { }, response);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Registration failed for {Email}: {Message}", request.Email, ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for {Email}", request.Email);
            return StatusCode(500, new { message = "An error occurred during registration" });
        }
    }

    /// <summary>
    /// Refresh JWT token using refresh token
    /// </summary>
    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _authService.RefreshTokenAsync(request);
            
            _logger.LogInformation("Token refreshed successfully");
            
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Token refresh failed: {Message}", ex.Message);
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            return StatusCode(500, new { message = "An error occurred during token refresh" });
        }
    }

    /// <summary>
    /// Logout user and invalidate token
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult> Logout()
    {
        try
        {
            var token = GetTokenFromHeader();
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new { message = "Token not found" });
            }

            var success = await _authService.LogoutAsync(token);
            
            if (success)
            {
                _logger.LogInformation("User logged out successfully");
                return Ok(new { message = "Logged out successfully" });
            }
            
            return BadRequest(new { message = "Logout failed" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return StatusCode(500, new { message = "An error occurred during logout" });
        }
    }

    /// <summary>
    /// Get current user information
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserInfo>> GetUserInfo()
    {
        try
        {
            var token = GetTokenFromHeader();
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new { message = "Token not found" });
            }

            var userInfo = await _authService.GetUserInfoAsync(token);
            
            return Ok(userInfo);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Get user info failed: {Message}", ex.Message);
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user info");
            return StatusCode(500, new { message = "An error occurred while getting user info" });
        }
    }

    /// <summary>
    /// Change user password
    /// </summary>
    [HttpPost("change-password")]
    [Authorize]
    public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetUserIdFromToken();
            if (userId == 0)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var success = await _authService.ChangePasswordAsync(userId, request);
            
            if (success)
            {
                _logger.LogInformation("Password changed successfully for user {UserId}", userId);
                return Ok(new { message = "Password changed successfully" });
            }
            
            return BadRequest(new { message = "Current password is incorrect" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password");
            return StatusCode(500, new { message = "An error occurred while changing password" });
        }
    }

    /// <summary>
    /// Request password reset
    /// </summary>
    [HttpPost("forgot-password")]
    public async Task<ActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _authService.ForgotPasswordAsync(request);
            
            _logger.LogInformation("Password reset requested for {Email}", request.Email);
            
            // Always return success to prevent email enumeration
            return Ok(new { message = "If the email exists, a password reset link has been sent" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error requesting password reset for {Email}", request.Email);
            return StatusCode(500, new { message = "An error occurred while processing the request" });
        }
    }

    /// <summary>
    /// Reset password using reset token
    /// </summary>
    [HttpPost("reset-password")]
    public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _authService.ResetPasswordAsync(request);
            
            if (success)
            {
                _logger.LogInformation("Password reset successfully for {Email}", request.Email);
                return Ok(new { message = "Password reset successfully" });
            }
            
            return BadRequest(new { message = "Invalid or expired reset token" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting password for {Email}", request.Email);
            return StatusCode(500, new { message = "An error occurred while resetting password" });
        }
    }

    /// <summary>
    /// Validate JWT token
    /// </summary>
    [HttpPost("validate")]
    public async Task<ActionResult> ValidateToken()
    {
        try
        {
            var token = GetTokenFromHeader();
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new { message = "Token not found" });
            }

            var isValid = await _authService.ValidateTokenAsync(token);
            
            if (isValid)
            {
                return Ok(new { valid = true, message = "Token is valid" });
            }
            
            return Unauthorized(new { valid = false, message = "Token is invalid or expired" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating token");
            return StatusCode(500, new { message = "An error occurred while validating token" });
        }
    }

    /// <summary>
    /// Get user permissions
    /// </summary>
    [HttpGet("permissions")]
    [Authorize]
    public async Task<ActionResult<List<string>>> GetPermissions()
    {
        try
        {
            var userId = GetUserIdFromToken();
            if (userId == 0)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var permissions = await _authService.GetUserPermissionsAsync(userId);
            
            return Ok(permissions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user permissions");
            return StatusCode(500, new { message = "An error occurred while getting permissions" });
        }
    }

    /// <summary>
    /// Check if user has specific permission
    /// </summary>
    [HttpPost("check-permission")]
    [Authorize]
    public async Task<ActionResult> CheckPermission([FromBody] CheckPermissionRequest request)
    {
        try
        {
            var userId = GetUserIdFromToken();
            if (userId == 0)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var hasPermission = await _authService.HasPermissionAsync(userId, request.Permission);
            
            return Ok(new { hasPermission, permission = request.Permission });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking permission");
            return StatusCode(500, new { message = "An error occurred while checking permission" });
        }
    }

    private string GetTokenFromHeader()
    {
        var authHeader = Request.Headers["Authorization"].FirstOrDefault();
        if (authHeader?.StartsWith("Bearer ") == true)
        {
            return authHeader.Substring(7);
        }
        return string.Empty;
    }

    private int GetUserIdFromToken()
    {
        var token = GetTokenFromHeader();
        if (string.IsNullOrEmpty(token))
            return 0;

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(token);
            var userIdClaim = jsonToken.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : 0;
        }
        catch
        {
            return 0;
        }
    }
}

public class CheckPermissionRequest
{
    public string Permission { get; set; } = string.Empty;
}
