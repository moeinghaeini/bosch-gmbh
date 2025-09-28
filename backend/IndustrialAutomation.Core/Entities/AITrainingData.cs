namespace IndustrialAutomation.Core.Entities;

public class AITrainingData : BaseEntity
{
    public string DataType { get; set; } = string.Empty; // TestResults, WebElements, Logs, etc.
    public string Content { get; set; } = string.Empty;
    public string DataContent { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string Quality { get; set; } = "Good"; // Poor, Fair, Good, Excellent
    public double QualityScore { get; set; } = 0.0;
    public bool IsValidated { get; set; } = false;
    public string ValidationNotes { get; set; } = string.Empty;
    public string Tags { get; set; } = string.Empty;
    public string Metadata { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
