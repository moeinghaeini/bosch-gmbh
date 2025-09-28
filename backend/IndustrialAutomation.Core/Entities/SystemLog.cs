using System.ComponentModel.DataAnnotations;

namespace IndustrialAutomation.Core.Entities;

public class SystemLog : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string Level { get; set; } = string.Empty; // Info, Warning, Error, Debug
    
    [Required]
    [MaxLength(200)]
    public string Message { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string Source { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string UserId { get; set; } = string.Empty;
    
    public string Exception { get; set; } = string.Empty;
    
    public string Properties { get; set; } = string.Empty; // JSON string
    
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
