namespace IndustrialAutomation.Core.Entities;

public class AutomationJob : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int StatusId { get; set; } = 1; // Foreign key to status lookup
    public int JobTypeId { get; set; } = 1; // Foreign key to job type lookup
    public DateTime? ScheduledAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? ErrorMessage { get; set; }
    public string Configuration { get; set; } = string.Empty; // JSON configuration
    public int? ExecutionTimeMs { get; set; }
    public int RetryCount { get; set; } = 0;
    public int MaxRetries { get; set; } = 3;
    public int Priority { get; set; } = 1;
    public string? CreatedBy { get; set; }
    public string? AssignedTo { get; set; }
}
