namespace IndustrialAutomation.Core.Entities;

public class WebAutomation : BaseEntity
{
    public string AutomationName { get; set; } = string.Empty;
    public string WebsiteUrl { get; set; } = string.Empty;
    public int StatusId { get; set; } = 1; // Foreign key to status lookup
    public int JobTypeId { get; set; } = 1; // Foreign key to job type lookup
    public string Status { get; set; } = string.Empty; // Status name for compatibility
    public string AutomationType { get; set; } = string.Empty; // Automation type for compatibility
    public string TargetElement { get; set; } = string.Empty; // AI-identified element
    public string Action { get; set; } = string.Empty; // Click, Type, Select, Navigate
    public string InputData { get; set; } = string.Empty; // JSON input data
    public string OutputData { get; set; } = string.Empty; // JSON extracted data
    public string AIPrompt { get; set; } = string.Empty; // Natural language prompt
    public string AIResponse { get; set; } = string.Empty; // Response data
    public string ElementSelector { get; set; } = string.Empty; // Element selector
    public string ScreenshotPath { get; set; } = string.Empty;
    public string VideoPath { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public int? ExecutionTimeMs { get; set; }
    public int? ExecutionTime { get; set; } // For compatibility
    public string Browser { get; set; } = "Chrome";
    public string Device { get; set; } = "Desktop";
    public string UserAgent { get; set; } = string.Empty;
    public string ViewportSize { get; set; } = "1920x1080";
    public string ConfidenceScore { get; set; } = string.Empty;
    public string? CreatedBy { get; set; }
}
