namespace IndustrialAutomation.Core.Entities;

public class PerformanceBenchmark : BaseEntity
{
    public string BenchmarkName { get; set; } = string.Empty;
    public string BenchmarkType { get; set; } = string.Empty; // LoadTest, StressTest, PerformanceTest
    public string Description { get; set; } = string.Empty;
    public string Configuration { get; set; } = string.Empty; // JSON test configuration
    public string TestData { get; set; } = string.Empty; // JSON test data
    public string Results { get; set; } = string.Empty; // JSON benchmark results
    public double AverageResponseTime { get; set; }
    public double MaxResponseTime { get; set; }
    public double MinResponseTime { get; set; }
    public double Throughput { get; set; } // Requests per second
    public double ErrorRate { get; set; }
    public double CpuUsage { get; set; }
    public double MemoryUsage { get; set; }
    public double DiskUsage { get; set; }
    public int ConcurrentUsers { get; set; }
    public int TotalRequests { get; set; }
    public int SuccessfulRequests { get; set; }
    public int FailedRequests { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Running, Completed, Failed
    public string Environment { get; set; } = string.Empty; // Development, Staging, Production
    public string CreatedBy { get; set; } = string.Empty;
    public string Tags { get; set; } = string.Empty;
}
