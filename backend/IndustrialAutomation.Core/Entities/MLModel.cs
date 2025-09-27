namespace IndustrialAutomation.Core.Entities;

public class MLModel : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ModelType { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Status { get; set; } = "Draft"; // Draft, Training, Ready, Deployed, Retired
    public string FilePath { get; set; } = string.Empty;
    public string Configuration { get; set; } = string.Empty;
    public double Accuracy { get; set; }
    public double Precision { get; set; }
    public double Recall { get; set; }
    public double F1Score { get; set; }
    public string TrainingData { get; set; } = string.Empty;
    public DateTime? TrainedAt { get; set; }
    public DateTime? DeployedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string Tags { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
