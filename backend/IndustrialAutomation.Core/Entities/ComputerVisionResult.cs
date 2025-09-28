namespace IndustrialAutomation.Core.Entities;

public class ComputerVisionResult : BaseEntity
{
    public string ImagePath { get; set; } = string.Empty;
    public string WebsiteUrl { get; set; } = string.Empty;
    public string ElementType { get; set; } = string.Empty; // Button, Input, Link, Image, etc.
    public string ElementDescription { get; set; } = string.Empty;
    public string BoundingBox { get; set; } = string.Empty; // JSON coordinates
    public string ConfidenceScore { get; set; } = string.Empty;
    public string Selector { get; set; } = string.Empty; // CSS selector or XPath
    public string Attributes { get; set; } = string.Empty; // JSON element attributes
    public string Text { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string Visibility { get; set; } = string.Empty; // Visible, Hidden, PartiallyVisible
    public string Accessibility { get; set; } = string.Empty; // JSON accessibility info
    public string Action { get; set; } = string.Empty; // Click, Type, Select, etc.
    public string InputData { get; set; } = string.Empty; // JSON input data
    public string ValidationRules { get; set; } = string.Empty; // JSON validation rules
    public string Status { get; set; } = "Detected"; // Detected, Validated, Failed
    public string ErrorMessage { get; set; } = string.Empty;
    public DateTime? DetectedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string Tags { get; set; } = string.Empty;
}
