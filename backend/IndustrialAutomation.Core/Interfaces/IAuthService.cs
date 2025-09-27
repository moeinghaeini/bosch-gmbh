using IndustrialAutomation.Core.Models;

namespace IndustrialAutomation.Core.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request);
    Task<bool> LogoutAsync(string token);
    Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequest request);
    Task<bool> ForgotPasswordAsync(ForgotPasswordRequest request);
    Task<bool> ResetPasswordAsync(ResetPasswordRequest request);
    Task<bool> ValidateTokenAsync(string token);
    Task<UserInfo> GetUserInfoAsync(string token);
    Task<bool> RevokeTokenAsync(string token);
    Task<List<string>> GetUserPermissionsAsync(int userId);
    Task<bool> HasPermissionAsync(int userId, string permission);
    Task<bool> IsInRoleAsync(int userId, string role);
}
