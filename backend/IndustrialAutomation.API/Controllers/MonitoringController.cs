using IndustrialAutomation.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IndustrialAutomation.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MonitoringController : ControllerBase
{
    private readonly IMonitoringService _monitoringService;
    private readonly ILogger<MonitoringController> _logger;

    public MonitoringController(IMonitoringService monitoringService, ILogger<MonitoringController> logger)
    {
        _monitoringService = monitoringService;
        _logger = logger;
    }

    /// <summary>
    /// Get system health status
    /// </summary>
    [HttpGet("health")]
    public async Task<ActionResult<Dictionary<string, object>>> GetSystemHealth()
    {
        try
        {
            var health = await _monitoringService.GetSystemHealthAsync();
            return Ok(health);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting system health");
            return StatusCode(500, new { message = "Error getting system health" });
        }
    }

    /// <summary>
    /// Get performance metrics
    /// </summary>
    [HttpGet("performance")]
    public async Task<ActionResult<Dictionary<string, object>>> GetPerformanceMetrics()
    {
        try
        {
            var metrics = await _monitoringService.GetPerformanceMetricsAsync();
            return Ok(metrics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting performance metrics");
            return StatusCode(500, new { message = "Error getting performance metrics" });
        }
    }

    /// <summary>
    /// Get business metrics
    /// </summary>
    [HttpGet("business")]
    public async Task<ActionResult<Dictionary<string, object>>> GetBusinessMetrics()
    {
        try
        {
            var metrics = await _monitoringService.GetBusinessMetricsAsync();
            return Ok(metrics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting business metrics");
            return StatusCode(500, new { message = "Error getting business metrics" });
        }
    }

    /// <summary>
    /// Get active alerts
    /// </summary>
    [HttpGet("alerts")]
    public async Task<ActionResult<List<Dictionary<string, object>>>> GetAlerts()
    {
        try
        {
            var alerts = await _monitoringService.GetAlertsAsync();
            return Ok(alerts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting alerts");
            return StatusCode(500, new { message = "Error getting alerts" });
        }
    }

    /// <summary>
    /// Get comprehensive dashboard data
    /// </summary>
    [HttpGet("dashboard")]
    public async Task<ActionResult<Dictionary<string, object>>> GetDashboardData()
    {
        try
        {
            var dashboardData = await _monitoringService.GetDashboardDataAsync();
            return Ok(dashboardData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting dashboard data");
            return StatusCode(500, new { message = "Error getting dashboard data" });
        }
    }

    /// <summary>
    /// Get system logs with filtering
    /// </summary>
    [HttpGet("logs")]
    public async Task<ActionResult<List<Dictionary<string, object>>>> GetLogs(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] string? level = null,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null)
    {
        try
        {
            var logs = await _monitoringService.GetLogsAsync(page, pageSize, level, from, to);
            return Ok(logs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting logs");
            return StatusCode(500, new { message = "Error getting logs" });
        }
    }

    /// <summary>
    /// Get trace information
    /// </summary>
    [HttpGet("traces/{traceId}")]
    public async Task<ActionResult<Dictionary<string, object>>> GetTrace(string traceId)
    {
        try
        {
            var trace = await _monitoringService.GetTraceAsync(traceId);
            return Ok(trace);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting trace {TraceId}", traceId);
            return StatusCode(500, new { message = "Error getting trace" });
        }
    }

    /// <summary>
    /// Record a custom metric
    /// </summary>
    [HttpPost("metrics")]
    public async Task<ActionResult> RecordMetric([FromBody] RecordMetricRequest request)
    {
        try
        {
            await _monitoringService.RecordMetricAsync(request.MetricName, request.Value, request.Tags);
            return Ok(new { message = "Metric recorded successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording metric");
            return StatusCode(500, new { message = "Error recording metric" });
        }
    }

    /// <summary>
    /// Record a custom event
    /// </summary>
    [HttpPost("events")]
    public async Task<ActionResult> RecordEvent([FromBody] RecordEventRequest request)
    {
        try
        {
            await _monitoringService.RecordEventAsync(request.EventName, request.Properties);
            return Ok(new { message = "Event recorded successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording event");
            return StatusCode(500, new { message = "Error recording event" });
        }
    }

    /// <summary>
    /// Record an error
    /// </summary>
    [HttpPost("errors")]
    public async Task<ActionResult> RecordError([FromBody] RecordErrorRequest request)
    {
        try
        {
            await _monitoringService.RecordErrorAsync(request.ErrorMessage, null, request.Context);
            return Ok(new { message = "Error recorded successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording error");
            return StatusCode(500, new { message = "Error recording error" });
        }
    }

    /// <summary>
    /// Create a new alert
    /// </summary>
    [HttpPost("alerts")]
    public async Task<ActionResult> CreateAlert([FromBody] CreateAlertRequest request)
    {
        try
        {
            await _monitoringService.CreateAlertAsync(request.AlertName, request.Description, request.Severity, request.Conditions);
            return Ok(new { message = "Alert created successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating alert");
            return StatusCode(500, new { message = "Error creating alert" });
        }
    }

    /// <summary>
    /// Start a new trace
    /// </summary>
    [HttpPost("traces")]
    public async Task<ActionResult> StartTrace([FromBody] StartTraceRequest request)
    {
        try
        {
            await _monitoringService.StartTraceAsync(request.TraceName, request.Attributes);
            return Ok(new { message = "Trace started successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting trace");
            return StatusCode(500, new { message = "Error starting trace" });
        }
    }
}

public class RecordMetricRequest
{
    public string MetricName { get; set; } = string.Empty;
    public double Value { get; set; }
    public Dictionary<string, string>? Tags { get; set; }
}

public class RecordEventRequest
{
    public string EventName { get; set; } = string.Empty;
    public Dictionary<string, object>? Properties { get; set; }
}

public class RecordErrorRequest
{
    public string ErrorMessage { get; set; } = string.Empty;
    public Dictionary<string, string>? Context { get; set; }
}

public class CreateAlertRequest
{
    public string AlertName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Severity { get; set; } = "warning";
    public Dictionary<string, string>? Conditions { get; set; }
}

public class StartTraceRequest
{
    public string TraceName { get; set; } = string.Empty;
    public Dictionary<string, string>? Attributes { get; set; }
}
