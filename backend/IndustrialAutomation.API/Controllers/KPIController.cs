using Microsoft.AspNetCore.Mvc;
using IndustrialAutomation.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace BoschThesis.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Require authentication
public class KPIController : ControllerBase
{
    private readonly ITestExecutionRepository _testExecutionRepository;
    private readonly IWebAutomationRepository _webAutomationRepository;
    private readonly IJobScheduleRepository _jobScheduleRepository;
    private readonly ILogger<KPIController> _logger;

    public KPIController(
        ITestExecutionRepository testExecutionRepository,
        IWebAutomationRepository webAutomationRepository,
        IJobScheduleRepository jobScheduleRepository,
        ILogger<KPIController> logger)
    {
        _testExecutionRepository = testExecutionRepository;
        _webAutomationRepository = webAutomationRepository;
        _jobScheduleRepository = jobScheduleRepository;
        _logger = logger;
    }

    [HttpGet("test-execution")]
    public async Task<ActionResult<TestExecutionKPIs>> GetTestExecutionKPIs()
    {
        try
        {
            var testExecutions = await _testExecutionRepository.GetAllAsync();
            var totalTests = testExecutions.Count();
            var passedTests = testExecutions.Count(t => t.StatusId == 3); // Completed (passed)
            var failedTests = testExecutions.Count(t => t.StatusId == 4); // Failed
            var runningTests = testExecutions.Count(t => t.StatusId == 2); // Running

            var kpis = new TestExecutionKPIs
            {
                TotalTests = totalTests,
                PassedTests = passedTests,
                FailedTests = failedTests,
                RunningTests = runningTests,
                SuccessRate = totalTests > 0 ? Math.Round((double)passedTests / totalTests * 100, 2) : 0,
                FailureRate = totalTests > 0 ? Math.Round((double)failedTests / totalTests * 100, 2) : 0,
                AverageExecutionTime = testExecutions
                    .Where(t => t.ExecutionTime.HasValue)
                    .Average(t => t.ExecutionTime!.Value / 60.0), // Convert seconds to minutes
                TestsByType = testExecutions
                    .GroupBy(t => GetTestTypeName(t.TestTypeId))
                    .ToDictionary(g => g.Key, g => g.Count()),
                TestsByStatus = testExecutions
                    .GroupBy(t => GetStatusName(t.StatusId))
                    .ToDictionary(g => g.Key, g => g.Count()),
                RecentTrends = CalculateRecentTrends(testExecutions)
            };

            return Ok(kpis);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving test execution KPIs");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("web-automation")]
    public async Task<ActionResult<WebAutomationKPIs>> GetWebAutomationKPIs()
    {
        try
        {
            var webAutomations = await _webAutomationRepository.GetAllAsync();
            var totalAutomations = webAutomations.Count();
            var completedAutomations = webAutomations.Count(w => w.StatusId == 3); // Completed
            var failedAutomations = webAutomations.Count(w => w.StatusId == 4); // Failed
            var runningAutomations = webAutomations.Count(w => w.StatusId == 2); // Running

            var kpis = new WebAutomationKPIs
            {
                TotalAutomations = totalAutomations,
                CompletedAutomations = completedAutomations,
                FailedAutomations = failedAutomations,
                RunningAutomations = runningAutomations,
                SuccessRate = totalAutomations > 0 ? Math.Round((double)completedAutomations / totalAutomations * 100, 2) : 0,
                FailureRate = totalAutomations > 0 ? Math.Round((double)failedAutomations / totalAutomations * 100, 2) : 0,
                AverageExecutionTime = webAutomations
                    .Where(w => w.ExecutionTime.HasValue)
                    .Average(w => w.ExecutionTime!.Value / 60.0), // Convert seconds to minutes
                AutomationsByType = webAutomations
                    .GroupBy(w => GetJobTypeName(w.JobTypeId))
                    .ToDictionary(g => g.Key, g => g.Count()),
                AutomationsByStatus = webAutomations
                    .GroupBy(w => GetStatusName(w.StatusId))
                    .ToDictionary(g => g.Key, g => g.Count()),
                RecentTrends = CalculateWebAutomationTrends(webAutomations)
            };

            return Ok(kpis);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving web automation KPIs");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("job-scheduling")]
    public async Task<ActionResult<JobSchedulingKPIs>> GetJobSchedulingKPIs()
    {
        try
        {
            var jobSchedules = await _jobScheduleRepository.GetAllAsync();
            var totalJobs = jobSchedules.Count();
            var enabledJobs = jobSchedules.Count(j => j.IsEnabled);
            var scheduledJobs = jobSchedules.Count(j => j.StatusId == 1); // Pending (scheduled)
            var runningJobs = jobSchedules.Count(j => j.StatusId == 2); // Running
            var completedJobs = jobSchedules.Count(j => j.StatusId == 3); // Completed

            var kpis = new JobSchedulingKPIs
            {
                TotalJobs = totalJobs,
                EnabledJobs = enabledJobs,
                ScheduledJobs = scheduledJobs,
                RunningJobs = runningJobs,
                CompletedJobs = completedJobs,
                SuccessRate = totalJobs > 0 ? Math.Round((double)completedJobs / totalJobs * 100, 2) : 0,
                JobsByType = jobSchedules
                    .GroupBy(j => GetJobTypeName(j.JobTypeId))
                    .ToDictionary(g => g.Key, g => g.Count()),
                JobsByStatus = jobSchedules
                    .GroupBy(j => GetStatusName(j.StatusId))
                    .ToDictionary(g => g.Key, g => g.Count()),
                JobsByPriority = jobSchedules
                    .GroupBy(j => j.Priority)
                    .ToDictionary(g => g.Key, g => g.Count()),
                RecentTrends = CalculateJobSchedulingTrends(jobSchedules)
            };

            return Ok(kpis);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving job scheduling KPIs");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("overall-performance")]
    public async Task<ActionResult<OverallPerformanceKPIs>> GetOverallPerformanceKPIs()
    {
        try
        {
            var testExecutions = await _testExecutionRepository.GetAllAsync();
            var webAutomations = await _webAutomationRepository.GetAllAsync();
            var jobSchedules = await _jobScheduleRepository.GetAllAsync();

            var overallKpis = new OverallPerformanceKPIs
            {
                SystemUptime = 99.9, // Mock uptime percentage
                TotalAutomationTasks = testExecutions.Count() + webAutomations.Count() + jobSchedules.Count(),
                SuccessfulTasks = testExecutions.Count(t => t.StatusId == 3) + // Completed (passed)
                                 webAutomations.Count(w => w.StatusId == 3) + // Completed
                                 jobSchedules.Count(j => j.StatusId == 3), // Completed
                FailedTasks = testExecutions.Count(t => t.StatusId == 4) + // Failed
                             webAutomations.Count(w => w.StatusId == 4) + // Failed
                             jobSchedules.Count(j => j.StatusId == 4), // Failed
                AverageResponseTime = 150, // Mock response time in ms
                ThroughputPerHour = 25, // Mock throughput
                ErrorRate = 2.5, // Mock error rate percentage
                ResourceUtilization = new Dictionary<string, double>
                {
                    { "CPU", 65.5 },
                    { "Memory", 78.2 },
                    { "Disk", 45.8 },
                    { "Network", 32.1 }
                },
                PerformanceTrends = CalculateOverallTrends(testExecutions, webAutomations, jobSchedules)
            };

            return Ok(overallKpis);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving overall performance KPIs");
            return StatusCode(500, "Internal server error");
        }
    }

    private Dictionary<string, object> CalculateRecentTrends(IEnumerable<dynamic> testExecutions)
    {
        // Mock trend calculation
        return new Dictionary<string, object>
        {
            { "last7Days", new { success = 85, failure = 15 } },
            { "last30Days", new { success = 88, failure = 12 } },
            { "trend", "improving" }
        };
    }

    private Dictionary<string, object> CalculateWebAutomationTrends(IEnumerable<dynamic> webAutomations)
    {
        // Mock trend calculation
        return new Dictionary<string, object>
        {
            { "last7Days", new { success = 92, failure = 8 } },
            { "last30Days", new { success = 89, failure = 11 } },
            { "trend", "stable" }
        };
    }

    private Dictionary<string, object> CalculateJobSchedulingTrends(IEnumerable<dynamic> jobSchedules)
    {
        // Mock trend calculation
        return new Dictionary<string, object>
        {
            { "last7Days", new { success = 95, failure = 5 } },
            { "last30Days", new { success = 93, failure = 7 } },
            { "trend", "improving" }
        };
    }

    private Dictionary<string, object> CalculateOverallTrends(
        IEnumerable<dynamic> testExecutions, 
        IEnumerable<dynamic> webAutomations, 
        IEnumerable<dynamic> jobSchedules)
    {
        // Mock overall trend calculation
        return new Dictionary<string, object>
        {
            { "performance", "excellent" },
            { "reliability", "high" },
            { "efficiency", "optimized" },
            { "scalability", "good" }
        };
    }

    private string GetStatusName(int statusId)
    {
        return statusId switch
        {
            1 => "Pending",
            2 => "Running",
            3 => "Completed", 
            4 => "Failed",
            5 => "Cancelled",
            _ => "Unknown"
        };
    }

    private string GetTestTypeName(int testTypeId)
    {
        return testTypeId switch
        {
            1 => "Unit",
            2 => "Integration",
            3 => "E2E", 
            4 => "Performance",
            5 => "Security",
            6 => "UI",
            _ => "Unknown"
        };
    }

    private string GetJobTypeName(int jobTypeId)
    {
        return jobTypeId switch
        {
            1 => "Data Processing",
            2 => "Report Generation",
            3 => "System Maintenance",
            4 => "Backup",
            5 => "Web Scraping",
            6 => "API Testing",
            _ => "Unknown"
        };
    }
}

public class TestExecutionKPIs
{
    public int TotalTests { get; set; }
    public int PassedTests { get; set; }
    public int FailedTests { get; set; }
    public int RunningTests { get; set; }
    public double SuccessRate { get; set; }
    public double FailureRate { get; set; }
    public double AverageExecutionTime { get; set; }
    public Dictionary<string, int> TestsByType { get; set; } = new();
    public Dictionary<string, int> TestsByStatus { get; set; } = new();
    public Dictionary<string, object> RecentTrends { get; set; } = new();
}

public class WebAutomationKPIs
{
    public int TotalAutomations { get; set; }
    public int CompletedAutomations { get; set; }
    public int FailedAutomations { get; set; }
    public int RunningAutomations { get; set; }
    public double SuccessRate { get; set; }
    public double FailureRate { get; set; }
    public double AverageExecutionTime { get; set; }
    public Dictionary<string, int> AutomationsByType { get; set; } = new();
    public Dictionary<string, int> AutomationsByStatus { get; set; } = new();
    public Dictionary<string, object> RecentTrends { get; set; } = new();
}

public class JobSchedulingKPIs
{
    public int TotalJobs { get; set; }
    public int EnabledJobs { get; set; }
    public int ScheduledJobs { get; set; }
    public int RunningJobs { get; set; }
    public int CompletedJobs { get; set; }
    public double SuccessRate { get; set; }
    public Dictionary<string, int> JobsByType { get; set; } = new();
    public Dictionary<string, int> JobsByStatus { get; set; } = new();
    public Dictionary<string, int> JobsByPriority { get; set; } = new();
    public Dictionary<string, object> RecentTrends { get; set; } = new();
}

public class OverallPerformanceKPIs
{
    public double SystemUptime { get; set; }
    public int TotalAutomationTasks { get; set; }
    public int SuccessfulTasks { get; set; }
    public int FailedTasks { get; set; }
    public double AverageResponseTime { get; set; }
    public double ThroughputPerHour { get; set; }
    public double ErrorRate { get; set; }
    public Dictionary<string, double> ResourceUtilization { get; set; } = new();
    public Dictionary<string, object> PerformanceTrends { get; set; } = new();
}
