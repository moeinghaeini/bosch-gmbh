namespace IndustrialAutomation.Core.Entities;

public class UserRefreshToken : BaseEntity
{
    public int UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; } = false;
    public DateTime? RevokedAt { get; set; }
    public string? RevokedBy { get; set; }
    public string? RevokeReason { get; set; }
    
    // Navigation property
    public User User { get; set; } = null!;
}
