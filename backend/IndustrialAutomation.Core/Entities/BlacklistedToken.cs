namespace IndustrialAutomation.Core.Entities;

public class BlacklistedToken : BaseEntity
{
    public string Jti { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public string? Reason { get; set; }
}
