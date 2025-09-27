namespace IndustrialAutomation.Core.Entities;

public class JobSchedule : BaseEntity
{
    public string JobName { get; set; } = string.Empty;
    public string JobType { get; set; } = string.Empty; // TestExecution, WebAutomation, DataProcessing
    public string Status { get; set; } = "Scheduled"; // Scheduled, Running, Completed, Failed, Cancelled
    public string CronExpression { get; set; } = string.Empty;
    public DateTime? NextRunTime { get; set; }
    public DateTime? LastRunTime { get; set; }
    public string Configuration { get; set; } = string.Empty; // JSON configuration
    public string Priority { get; set; } = "Normal"; // Low, Normal, High, Critical
    public bool IsEnabled { get; set; } = true;
    public string TimeZone { get; set; } = "UTC";
    public int MaxRetries { get; set; } = 3;
    public int CurrentRetries { get; set; } = 0;
    public string ErrorMessage { get; set; } = string.Empty;
    public string ExecutionHistory { get; set; } = string.Empty; // JSON execution history
    public string Notifications { get; set; } = string.Empty; // JSON notification settings
    public string Dependencies { get; set; } = string.Empty; // JSON job dependencies
}
