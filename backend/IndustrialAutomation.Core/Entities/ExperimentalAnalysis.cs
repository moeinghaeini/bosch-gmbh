namespace IndustrialAutomation.Core.Entities;

public class ExperimentalAnalysis : BaseEntity
{
    public string AnalysisName { get; set; } = string.Empty;
    public string AnalysisType { get; set; } = string.Empty; // TestExecution, WebAutomation, Performance
    public string Description { get; set; } = string.Empty;
    public string Methodology { get; set; } = string.Empty; // JSON methodology details
    public string Dataset { get; set; } = string.Empty; // JSON dataset information
    public string Results { get; set; } = string.Empty; // JSON experimental results
    public string Metrics { get; set; } = string.Empty; // JSON performance metrics
    public string StatisticalAnalysis { get; set; } = string.Empty; // JSON statistical analysis
    public string Conclusions { get; set; } = string.Empty;
    public string Recommendations { get; set; } = string.Empty;
    public double Accuracy { get; set; }
    public double Precision { get; set; }
    public double Recall { get; set; }
    public double F1Score { get; set; }
    public double ExecutionTime { get; set; }
    public double SuccessRate { get; set; }
    public int SampleSize { get; set; }
    public string Status { get; set; } = "Draft"; // Draft, Running, Completed, Published
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string Tags { get; set; } = string.Empty;
    public bool IsPublished { get; set; } = false;
}
