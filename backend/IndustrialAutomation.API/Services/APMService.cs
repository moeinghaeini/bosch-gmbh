using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace IndustrialAutomation.API.Services;

public interface IAPMService
{
    Task<PerformanceMetric> StartTraceAsync(string operationName, Dictionary<string, object>? tags = null);
    Task EndTraceAsync(string traceId, bool success = true, string? error = null);
    Task<Span> StartSpanAsync(string traceId, string spanName, Dictionary<string, object>? tags = null);
    Task EndSpanAsync(string spanId, bool success = true, string? error = null);
    Task<List<PerformanceMetric>> GetPerformanceMetricsAsync(DateTime from, DateTime to);
    Task<ApplicationHealth> GetApplicationHealthAsync();
    Task<List<ErrorRate>> GetErrorRatesAsync(DateTime from, DateTime to);
    Task<List<ThroughputMetric>> GetThroughputMetricsAsync(DateTime from, DateTime to);
    Task<LatencyAnalysis> AnalyzeLatencyAsync(string operationName, DateTime from, DateTime to);
    Task<List<DependencyMetric>> GetDependencyMetricsAsync(DateTime from, DateTime to);
    Task<AlertRule> CreateAlertRuleAsync(AlertRule rule);
    Task<List<Alert>> GetActiveAlertsAsync();
    Task<bool> AcknowledgeAlertAsync(string alertId);
    Task<Dashboard> CreateDashboardAsync(Dashboard dashboard);
    Task<List<Dashboard>> GetDashboardsAsync();
}

public class APMService : IAPMService
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private readonly ILogger<APMService> _logger;
    private readonly ActivitySource _activitySource;

    public APMService(HttpClient httpClient, IMemoryCache cache, ILogger<APMService> logger)
    {
        _httpClient = httpClient;
        _cache = cache;
        _logger = logger;
        _activitySource = new ActivitySource("IndustrialAutomation.APM");
    }

    public async Task<PerformanceMetric> StartTraceAsync(string operationName, Dictionary<string, object>? tags = null)
    {
        try
        {
            var traceId = Guid.NewGuid().ToString();
            var startTime = DateTime.UtcNow;
            
            var trace = new PerformanceMetric
            {
                TraceId = traceId,
                OperationName = operationName,
                StartTime = startTime,
                Tags = tags ?? new Dictionary<string, object>(),
                Status = "Running"
            };

            // Store in cache for quick access
            _cache.Set($"trace_{traceId}", trace, TimeSpan.FromHours(1));
            
            _logger.LogInformation("Started trace {TraceId} for operation {OperationName}", traceId, operationName);
            return trace;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting trace for operation {OperationName}", operationName);
            throw;
        }
    }

    public async Task EndTraceAsync(string traceId, bool success = true, string? error = null)
    {
        try
        {
            if (_cache.TryGetValue($"trace_{traceId}", out PerformanceMetric? trace))
            {
                trace.EndTime = DateTime.UtcNow;
                trace.Duration = (trace.EndTime - trace.StartTime).TotalMilliseconds;
                trace.Status = success ? "Completed" : "Failed";
                trace.Error = error;
                trace.Success = success;

                // Update cache
                _cache.Set($"trace_{traceId}", trace, TimeSpan.FromHours(1));
                
                _logger.LogInformation("Ended trace {TraceId} with status {Status}", traceId, trace.Status);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ending trace {TraceId}", traceId);
        }
    }

    public async Task<Span> StartSpanAsync(string traceId, string spanName, Dictionary<string, object>? tags = null)
    {
        try
        {
            var spanId = Guid.NewGuid().ToString();
            var startTime = DateTime.UtcNow;
            
            var span = new Span
            {
                SpanId = spanId,
                TraceId = traceId,
                Name = spanName,
                StartTime = startTime,
                Tags = tags ?? new Dictionary<string, object>(),
                Status = "Running"
            };

            // Store in cache
            _cache.Set($"span_{spanId}", span, TimeSpan.FromHours(1));
            
            _logger.LogDebug("Started span {SpanId} for trace {TraceId}", spanId, traceId);
            return span;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting span {SpanName} for trace {TraceId}", spanName, traceId);
            throw;
        }
    }

    public async Task EndSpanAsync(string spanId, bool success = true, string? error = null)
    {
        try
        {
            if (_cache.TryGetValue($"span_{spanId}", out Span? span))
            {
                span.EndTime = DateTime.UtcNow;
                span.Duration = (span.EndTime - span.StartTime).TotalMilliseconds;
                span.Status = success ? "Completed" : "Failed";
                span.Error = error;
                span.Success = success;

                // Update cache
                _cache.Set($"span_{spanId}", span, TimeSpan.FromHours(1));
                
                _logger.LogDebug("Ended span {SpanId} with status {Status}", spanId, span.Status);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ending span {SpanId}", spanId);
        }
    }

    public async Task<List<PerformanceMetric>> GetPerformanceMetricsAsync(DateTime from, DateTime to)
    {
        try
        {
            var cacheKey = $"performance_metrics_{from:yyyyMMddHH}_{to:yyyyMMddHH}";
            if (_cache.TryGetValue(cacheKey, out List<PerformanceMetric>? cachedMetrics))
            {
                return cachedMetrics ?? new List<PerformanceMetric>();
            }

            var response = await _httpClient.GetAsync($"/api/apm/metrics?from={from:O}&to={to:O}");
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var metrics = JsonSerializer.Deserialize<List<PerformanceMetric>>(result) ?? new List<PerformanceMetric>();
                
                _cache.Set(cacheKey, metrics, TimeSpan.FromMinutes(5));
                return metrics;
            }
            
            return new List<PerformanceMetric>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting performance metrics from {From} to {To}", from, to);
            return new List<PerformanceMetric>();
        }
    }

    public async Task<ApplicationHealth> GetApplicationHealthAsync()
    {
        try
        {
            var cacheKey = "application_health";
            if (_cache.TryGetValue(cacheKey, out ApplicationHealth? cachedHealth))
            {
                return cachedHealth ?? new ApplicationHealth();
            }

            var response = await _httpClient.GetAsync("/api/apm/health");
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var health = JsonSerializer.Deserialize<ApplicationHealth>(result) ?? new ApplicationHealth();
                
                _cache.Set(cacheKey, health, TimeSpan.FromMinutes(1));
                return health;
            }
            
            return new ApplicationHealth();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting application health");
            return new ApplicationHealth();
        }
    }

    public async Task<List<ErrorRate>> GetErrorRatesAsync(DateTime from, DateTime to)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/apm/error-rates?from={from:O}&to={to:O}");
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<ErrorRate>>(result) ?? new List<ErrorRate>();
            }
            
            return new List<ErrorRate>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting error rates from {From} to {To}", from, to);
            return new List<ErrorRate>();
        }
    }

    public async Task<List<ThroughputMetric>> GetThroughputMetricsAsync(DateTime from, DateTime to)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/apm/throughput?from={from:O}&to={to:O}");
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<ThroughputMetric>>(result) ?? new List<ThroughputMetric>();
            }
            
            return new List<ThroughputMetric>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting throughput metrics from {From} to {To}", from, to);
            return new List<ThroughputMetric>();
        }
    }

    public async Task<LatencyAnalysis> AnalyzeLatencyAsync(string operationName, DateTime from, DateTime to)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/apm/latency/{operationName}?from={from:O}&to={to:O}");
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<LatencyAnalysis>(result) ?? new LatencyAnalysis();
            }
            
            return new LatencyAnalysis();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing latency for operation {OperationName}", operationName);
            return new LatencyAnalysis();
        }
    }

    public async Task<List<DependencyMetric>> GetDependencyMetricsAsync(DateTime from, DateTime to)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/apm/dependencies?from={from:O}&to={to:O}");
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<DependencyMetric>>(result) ?? new List<DependencyMetric>();
            }
            
            return new List<DependencyMetric>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting dependency metrics from {From} to {To}", from, to);
            return new List<DependencyMetric>();
        }
    }

    public async Task<AlertRule> CreateAlertRuleAsync(AlertRule rule)
    {
        try
        {
            var json = JsonSerializer.Serialize(rule);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync("/api/apm/alert-rules", content);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<AlertRule>(result) ?? new AlertRule();
            }
            
            return new AlertRule();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating alert rule {RuleName}", rule.Name);
            return new AlertRule();
        }
    }

    public async Task<List<Alert>> GetActiveAlertsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/apm/alerts/active");
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<Alert>>(result) ?? new List<Alert>();
            }
            
            return new List<Alert>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active alerts");
            return new List<Alert>();
        }
    }

    public async Task<bool> AcknowledgeAlertAsync(string alertId)
    {
        try
        {
            var response = await _httpClient.PostAsync($"/api/apm/alerts/{alertId}/acknowledge", null);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error acknowledging alert {AlertId}", alertId);
            return false;
        }
    }

    public async Task<Dashboard> CreateDashboardAsync(Dashboard dashboard)
    {
        try
        {
            var json = JsonSerializer.Serialize(dashboard);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync("/api/apm/dashboards", content);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Dashboard>(result) ?? new Dashboard();
            }
            
            return new Dashboard();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating dashboard {DashboardName}", dashboard.Name);
            return new Dashboard();
        }
    }

    public async Task<List<Dashboard>> GetDashboardsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/apm/dashboards");
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<Dashboard>>(result) ?? new List<Dashboard>();
            }
            
            return new List<Dashboard>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting dashboards");
            return new List<Dashboard>();
        }
    }
}

// Data Models
public class PerformanceMetric
{
    public string TraceId { get; set; } = string.Empty;
    public string OperationName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public double Duration { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string? Error { get; set; }
    public Dictionary<string, object> Tags { get; set; } = new();
}

public class Span
{
    public string SpanId { get; set; } = string.Empty;
    public string TraceId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public double Duration { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string? Error { get; set; }
    public Dictionary<string, object> Tags { get; set; } = new();
}

public class ApplicationHealth
{
    public string Status { get; set; } = string.Empty;
    public double CpuUsage { get; set; }
    public double MemoryUsage { get; set; }
    public double DiskUsage { get; set; }
    public int ActiveConnections { get; set; }
    public int RequestRate { get; set; }
    public double ErrorRate { get; set; }
    public DateTime Timestamp { get; set; }
}

public class ErrorRate
{
    public string Operation { get; set; } = string.Empty;
    public double Rate { get; set; }
    public int ErrorCount { get; set; }
    public int TotalCount { get; set; }
    public DateTime Timestamp { get; set; }
}

public class ThroughputMetric
{
    public string Operation { get; set; } = string.Empty;
    public int RequestsPerSecond { get; set; }
    public int TotalRequests { get; set; }
    public DateTime Timestamp { get; set; }
}

public class LatencyAnalysis
{
    public string Operation { get; set; } = string.Empty;
    public double P50 { get; set; }
    public double P90 { get; set; }
    public double P95 { get; set; }
    public double P99 { get; set; }
    public double Average { get; set; }
    public double Max { get; set; }
    public double Min { get; set; }
    public DateTime Timestamp { get; set; }
}

public class DependencyMetric
{
    public string Service { get; set; } = string.Empty;
    public string Dependency { get; set; } = string.Empty;
    public double Latency { get; set; }
    public double ErrorRate { get; set; }
    public int RequestCount { get; set; }
    public DateTime Timestamp { get; set; }
}

public class AlertRule
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Metric { get; set; } = string.Empty;
    public string Condition { get; set; } = string.Empty;
    public double Threshold { get; set; }
    public string Severity { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class Alert
{
    public string Id { get; set; } = string.Empty;
    public string RuleId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime TriggeredAt { get; set; }
    public DateTime? AcknowledgedAt { get; set; }
    public string? AcknowledgedBy { get; set; }
}

public class Dashboard
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<DashboardWidget> Widgets { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class DashboardWidget
{
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public Dictionary<string, object> Configuration { get; set; } = new();
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}
