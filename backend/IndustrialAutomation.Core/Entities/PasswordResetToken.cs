namespace IndustrialAutomation.Core.Entities;

public class PasswordResetToken : BaseEntity
{
    public int UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; } = false;
    public DateTime? UsedAt { get; set; }
    public string? UsedBy { get; set; }
    
    // Navigation property
    public User User { get; set; } = null!;
}
