namespace IndustrialAutomation.Core.Entities;

public class AuditLog : BaseEntity
{
    public string RequestId { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string? UserRole { get; set; }
    public string Method { get; set; } = string.Empty;
    public string? Path { get; set; }
    public string? QueryString { get; set; }
    public string? RequestBody { get; set; }
    public int StatusCode { get; set; }
    public long Duration { get; set; }
    public long ResponseSize { get; set; }
    public string? UserAgent { get; set; }
    public string? IpAddress { get; set; }
    public string? Exception { get; set; }
}
