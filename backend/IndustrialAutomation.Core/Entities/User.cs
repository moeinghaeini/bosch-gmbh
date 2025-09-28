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
    public DateTime? PasswordChangedAt { get; set; }
    public string UserRoles { get; set; } = string.Empty;
    public DateTime? LockedUntil { get; set; }
    public int FailedLoginAttempts { get; set; } = 0;
}
