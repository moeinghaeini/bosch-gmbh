namespace IndustrialAutomation.Core.Entities;

public class TestExecution : BaseEntity
{
    public string TestName { get; set; } = string.Empty;
    public int TestTypeId { get; set; } = 1; // Foreign key to test type lookup
    public int StatusId { get; set; } = 1; // Foreign key to status lookup
    public string TestSuite { get; set; } = string.Empty;
    public string TestData { get; set; } = string.Empty; // JSON test data
    public string ExpectedResult { get; set; } = string.Empty;
    public string ActualResult { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public int? ExecutionTimeMs { get; set; }
    public string AIAnalysis { get; set; } = string.Empty; // Analysis results
    public string ConfidenceScore { get; set; } = string.Empty; // AI confidence level
    public string TestEnvironment { get; set; } = string.Empty;
    public string Browser { get; set; } = string.Empty;
    public string Device { get; set; } = string.Empty;
    public string ScreenshotPath { get; set; } = string.Empty;
    public string VideoPath { get; set; } = string.Empty;
    public string LogPath { get; set; } = string.Empty;
    public string? CreatedBy { get; set; }
    public string? ExecutedBy { get; set; }
}
