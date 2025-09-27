namespace IndustrialAutomation.Core.Entities;

public class User : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Salt { get; set; } = string.Empty;
    public string Role { get; set; } = "User"; // Admin, User, Viewer
    public bool IsActive { get; set; } = true;
    public bool IsEmailVerified { get; set; } = false;
    public DateTime? LastLoginAt { get; set; }
    public DateTime PasswordChangedAt { get; set; } = DateTime.UtcNow;
    public int FailedLoginAttempts { get; set; } = 0;
    public DateTime? LockedUntil { get; set; }
    public bool TwoFactorEnabled { get; set; } = false;
    public string? TwoFactorSecret { get; set; }
    
    // Navigation properties
    public List<UserRole> UserRoles { get; set; } = new();
    public List<UserPermission> UserPermissions { get; set; } = new();
    public List<UserRefreshToken> RefreshTokens { get; set; } = new();
    public List<PasswordResetToken> PasswordResetTokens { get; set; } = new();
}
