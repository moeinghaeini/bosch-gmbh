using System.ComponentModel.DataAnnotations;

namespace IndustrialAutomation.Core.Entities;

public class PerformanceMetric : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string MetricName { get; set; } = string.Empty;
    
    public double MetricValue { get; set; }
    
    [MaxLength(100)]
    public string Category { get; set; } = string.Empty;
    
    public string Tags { get; set; } = string.Empty; // JSON string
    
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
