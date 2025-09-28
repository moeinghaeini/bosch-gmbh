using IndustrialAutomation.Core.Interfaces;
using IndustrialAutomation.Core.Entities;
using IndustrialAutomation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;

namespace IndustrialAutomation.Infrastructure.Services;

public class MonitoringService : IMonitoringService
{
    private readonly IndustrialAutomationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<MonitoringService> _logger;
    private readonly Dictionary<string, object> _metrics = new();
    private readonly Dictionary<string, DateTime> _traces = new();

    public MonitoringService(
        IndustrialAutomationDbContext context,
        IConfiguration configuration,
        ILogger<MonitoringService> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task RecordMetricAsync(string metricName, double value, Dictionary<string, string>? tags = null)
    {
        try
        {
            var metric = new IndustrialAutomation.Core.Entities.PerformanceMetric
            {
                MetricName = metricName,
                MetricValue = value,
                Category = "Custom",
                Tags = tags != null ? JsonSerializer.Serialize(tags) : string.Empty,
                Timestamp = DateTime.UtcNow
            };

            _context.PerformanceMetrics.Add(metric);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Recorded metric {MetricName}: {Value}", metricName, value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording metric {MetricName}", metricName);
        }
    }

    public async Task RecordCounterAsync(string counterName, int increment = 1, Dictionary<string, string>? tags = null)
    {
        await RecordMetricAsync($"counter.{counterName}", increment, tags);
    }

    public async Task RecordTimerAsync(string timerName, TimeSpan duration, Dictionary<string, string>? tags = null)
    {
        await RecordMetricAsync($"timer.{timerName}", duration.TotalMilliseconds, tags);
    }

    public async Task RecordGaugeAsync(string gaugeName, double value, Dictionary<string, string>? tags = null)
    {
        await RecordMetricAsync($"gauge.{gaugeName}", value, tags);
    }

    public async Task RecordHistogramAsync(string histogramName, double value, Dictionary<string, string>? tags = null)
    {
        await RecordMetricAsync($"histogram.{histogramName}", value, tags);
    }

    public async Task<Dictionary<string, object>> GetSystemHealthAsync()
    {
        try
        {
            var health = new Dictionary<string, object>
            {
                ["status"] = "healthy",
                ["timestamp"] = DateTime.UtcNow,
                ["version"] = "1.0.0",
                ["uptime"] = Environment.TickCount64 / 1000.0
            };

            // Check database connectivity
            try
            {
                await _context.Database.CanConnectAsync();
                health["database"] = "connected";
            }
            catch (Exception ex)
            {
                health["database"] = "disconnected";
                health["database_error"] = ex.Message;
                health["status"] = "unhealthy";
            }

            // Check memory usage
            var process = Process.GetCurrentProcess();
            health["memory_usage_mb"] = process.WorkingSet64 / 1024.0 / 1024.0;
            health["cpu_usage_percent"] = GetCpuUsage();

            // Check recent errors
            var recentErrors = await _context.SystemLogs
                .Where(log => log.Level == "ERROR" && log.Timestamp > DateTime.UtcNow.AddHours(-1))
                .CountAsync();

            health["recent_errors"] = recentErrors;
            if (recentErrors > 10)
            {
                health["status"] = "degraded";
            }

            return health;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting system health");
            return new Dictionary<string, object>
            {
                ["status"] = "error",
                ["error"] = ex.Message,
                ["timestamp"] = DateTime.UtcNow
            };
        }
    }

    public async Task<Dictionary<string, object>> GetPerformanceMetricsAsync()
    {
        try
        {
            var metrics = new Dictionary<string, object>();

            // Get metrics from last hour
            var oneHourAgo = DateTime.UtcNow.AddHours(-1);
            var recentMetrics = await _context.PerformanceMetrics
                .Where(m => m.Timestamp > oneHourAgo)
                .ToListAsync();

            // Group by metric name and calculate statistics
            var groupedMetrics = recentMetrics.GroupBy(m => m.MetricName);

            foreach (var group in groupedMetrics)
            {
                var values = group.Select(m => m.MetricValue).ToList();
                metrics[group.Key] = new Dictionary<string, object>
                {
                    ["count"] = values.Count,
                    ["min"] = values.Min(),
                    ["max"] = values.Max(),
                    ["avg"] = values.Average(),
                    ["sum"] = values.Sum(),
                    ["p95"] = CalculatePercentile(values, 0.95),
                    ["p99"] = CalculatePercentile(values, 0.99)
                };
            }

            // Add system metrics
            var process = Process.GetCurrentProcess();
            metrics["system"] = new Dictionary<string, object>
            {
                ["memory_usage_mb"] = process.WorkingSet64 / 1024.0 / 1024.0,
                ["cpu_usage_percent"] = GetCpuUsage(),
                ["thread_count"] = process.Threads.Count,
                ["handle_count"] = process.HandleCount
            };

            return metrics;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting performance metrics");
            return new Dictionary<string, object> { ["error"] = ex.Message };
        }
    }

    public async Task<Dictionary<string, object>> GetBusinessMetricsAsync()
    {
        try
        {
            var metrics = new Dictionary<string, object>();

            // Automation Jobs metrics
            var totalJobs = await _context.AutomationJobs.CountAsync();
            var completedJobs = await _context.AutomationJobs.CountAsync(j => j.StatusId == 3); // Completed
            var failedJobs = await _context.AutomationJobs.CountAsync(j => j.StatusId == 4); // Failed
            var runningJobs = await _context.AutomationJobs.CountAsync(j => j.StatusId == 2); // Running

            metrics["automation_jobs"] = new Dictionary<string, object>
            {
                ["total"] = totalJobs,
                ["completed"] = completedJobs,
                ["failed"] = failedJobs,
                ["running"] = runningJobs,
                ["success_rate"] = totalJobs > 0 ? (double)completedJobs / totalJobs * 100 : 0
            };

            // Test Executions metrics
            var totalTests = await _context.TestExecutions.CountAsync();
            var passedTests = await _context.TestExecutions.CountAsync(t => t.StatusId == 3); // Completed (passed)
            var failedTests = await _context.TestExecutions.CountAsync(t => t.StatusId == 4); // Failed

            metrics["test_executions"] = new Dictionary<string, object>
            {
                ["total"] = totalTests,
                ["passed"] = passedTests,
                ["failed"] = failedTests,
                ["pass_rate"] = totalTests > 0 ? (double)passedTests / totalTests * 100 : 0
            };

            // Web Automations metrics
            var totalWebAutomations = await _context.WebAutomations.CountAsync();
            var completedWebAutomations = await _context.WebAutomations.CountAsync(w => w.StatusId == 3); // Completed
            var failedWebAutomations = await _context.WebAutomations.CountAsync(w => w.StatusId == 4); // Failed

            metrics["web_automations"] = new Dictionary<string, object>
            {
                ["total"] = totalWebAutomations,
                ["completed"] = completedWebAutomations,
                ["failed"] = failedWebAutomations,
                ["success_rate"] = totalWebAutomations > 0 ? (double)completedWebAutomations / totalWebAutomations * 100 : 0
            };

            // Active Users
            var activeUsers = await _context.Users.CountAsync(u => u.IsActive && !u.IsDeleted);
            metrics["active_users"] = activeUsers;

            // Recent activity (last 24 hours)
            var yesterday = DateTime.UtcNow.AddDays(-1);
            var recentJobs = await _context.AutomationJobs.CountAsync(j => j.CreatedAt > yesterday);
            var recentTests = await _context.TestExecutions.CountAsync(t => t.CreatedAt > yesterday);

            metrics["recent_activity"] = new Dictionary<string, object>
            {
                ["jobs_created"] = recentJobs,
                ["tests_executed"] = recentTests
            };

            return metrics;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting business metrics");
            return new Dictionary<string, object> { ["error"] = ex.Message };
        }
    }

    public async Task<List<Dictionary<string, object>>> GetAlertsAsync()
    {
        try
        {
            var alerts = new List<Dictionary<string, object>>();

            // Check for high error rates
            var recentErrors = await _context.SystemLogs
                .Where(log => log.Level == "ERROR" && log.Timestamp > DateTime.UtcNow.AddHours(-1))
                .CountAsync();

            if (recentErrors > 5)
            {
                alerts.Add(new Dictionary<string, object>
                {
                    ["name"] = "High Error Rate",
                    ["severity"] = "warning",
                    ["message"] = $"High error rate detected: {recentErrors} errors in the last hour",
                    ["timestamp"] = DateTime.UtcNow
                });
            }

            // Check for failed jobs
            var failedJobs = await _context.AutomationJobs
                .Where(j => j.StatusId == 4 && j.CreatedAt > DateTime.UtcNow.AddHours(-1)) // StatusId 4 = "Failed"
                .CountAsync();

            if (failedJobs > 3)
            {
                alerts.Add(new Dictionary<string, object>
                {
                    ["name"] = "High Job Failure Rate",
                    ["severity"] = "critical",
                    ["message"] = $"High job failure rate: {failedJobs} failed jobs in the last hour",
                    ["timestamp"] = DateTime.UtcNow
                });
            }

            // Check for system resource usage
            var process = Process.GetCurrentProcess();
            var memoryUsageMB = process.WorkingSet64 / 1024.0 / 1024.0;

            if (memoryUsageMB > 1000) // 1GB threshold
            {
                alerts.Add(new Dictionary<string, object>
                {
                    ["name"] = "High Memory Usage",
                    ["severity"] = "warning",
                    ["message"] = $"High memory usage detected: {memoryUsageMB:F2} MB",
                    ["timestamp"] = DateTime.UtcNow
                });
            }

            return alerts;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting alerts");
            return new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>
                {
                    ["name"] = "Monitoring Error",
                    ["severity"] = "error",
                    ["message"] = ex.Message,
                    ["timestamp"] = DateTime.UtcNow
                }
            };
        }
    }

    public async Task CreateAlertAsync(string alertName, string description, string severity, Dictionary<string, string>? conditions = null)
    {
        try
        {
            var alert = new SystemLog
            {
                Level = "ALERT",
                Message = $"Alert created: {alertName} - {description}",
                Source = "MonitoringService",
                Timestamp = DateTime.UtcNow
            };

            _context.SystemLogs.Add(alert);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created alert: {AlertName}", alertName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating alert {AlertName}", alertName);
        }
    }

    public async Task UpdateAlertAsync(string alertName, Dictionary<string, object> updates)
    {
        try
        {
            var alert = new SystemLog
            {
                Level = "ALERT",
                Message = $"Alert updated: {alertName}",
                Source = "MonitoringService",
                Timestamp = DateTime.UtcNow
            };

            _context.SystemLogs.Add(alert);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Updated alert: {AlertName}", alertName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating alert {AlertName}", alertName);
        }
    }

    public async Task DeleteAlertAsync(string alertName)
    {
        try
        {
            var alert = new SystemLog
            {
                Level = "ALERT",
                Message = $"Alert deleted: {alertName}",
                Source = "MonitoringService",
                Timestamp = DateTime.UtcNow
            };

            _context.SystemLogs.Add(alert);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Deleted alert: {AlertName}", alertName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting alert {AlertName}", alertName);
        }
    }

    public async Task RecordEventAsync(string eventName, Dictionary<string, object>? properties = null)
    {
        try
        {
            var eventLog = new SystemLog
            {
                Level = "INFO",
                Message = $"Event: {eventName}",
                Source = "MonitoringService",
                Timestamp = DateTime.UtcNow
            };

            if (properties != null)
            {
                eventLog.Exception = JsonSerializer.Serialize(properties);
            }

            _context.SystemLogs.Add(eventLog);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Recorded event: {EventName}", eventName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording event {EventName}", eventName);
        }
    }

    public async Task RecordErrorAsync(string errorMessage, Exception? exception = null, Dictionary<string, string>? context = null)
    {
        try
        {
            var errorLog = new SystemLog
            {
                Level = "ERROR",
                Message = errorMessage,
                Exception = exception?.ToString(),
                Source = "MonitoringService",
                Timestamp = DateTime.UtcNow
            };

            if (context != null)
            {
                errorLog.Exception = JsonSerializer.Serialize(context);
            }

            _context.SystemLogs.Add(errorLog);
            await _context.SaveChangesAsync();

            _logger.LogError(exception, "Recorded error: {ErrorMessage}", errorMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording error log");
        }
    }

    public async Task RecordCustomEventAsync(string eventName, string category, Dictionary<string, object>? properties = null)
    {
        try
        {
            var customEvent = new SystemLog
            {
                Level = "INFO",
                Message = $"Custom Event [{category}]: {eventName}",
                Source = "MonitoringService",
                Timestamp = DateTime.UtcNow
            };

            if (properties != null)
            {
                customEvent.Exception = JsonSerializer.Serialize(properties);
            }

            _context.SystemLogs.Add(customEvent);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Recorded custom event: {EventName} in category {Category}", eventName, category);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording custom event {EventName}", eventName);
        }
    }

    public async Task<Dictionary<string, object>> GetDashboardDataAsync()
    {
        try
        {
            var health = await GetSystemHealthAsync();
            var performance = await GetPerformanceMetricsAsync();
            var business = await GetBusinessMetricsAsync();
            var alerts = await GetAlertsAsync();

            return new Dictionary<string, object>
            {
                ["health"] = health,
                ["performance"] = performance,
                ["business"] = business,
                ["alerts"] = alerts,
                ["timestamp"] = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting dashboard data");
            return new Dictionary<string, object> { ["error"] = ex.Message };
        }
    }

    public async Task<List<Dictionary<string, object>>> GetLogsAsync(int page = 1, int pageSize = 50, string? level = null, DateTime? from = null, DateTime? to = null)
    {
        try
        {
            var query = _context.SystemLogs.AsQueryable();

            if (!string.IsNullOrEmpty(level))
            {
                query = query.Where(log => log.Level == level);
            }

            if (from.HasValue)
            {
                query = query.Where(log => log.Timestamp >= from.Value);
            }

            if (to.HasValue)
            {
                query = query.Where(log => log.Timestamp <= to.Value);
            }

            var logs = await query
                .OrderByDescending(log => log.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(log => new { 
                    Id = log.Id,
                    Level = log.Level,
                    Message = log.Message,
                    Source = log.Source,
                    Timestamp = log.Timestamp,
                    Exception = log.Exception
                })
                .ToListAsync();

            var result = logs.Select(log => new Dictionary<string, object>
            {
                ["id"] = log.Id,
                ["level"] = log.Level,
                ["message"] = log.Message,
                ["source"] = log.Source,
                ["timestamp"] = log.Timestamp,
                ["exception"] = log.Exception
            }).ToList();

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting logs");
            return new List<Dictionary<string, object>>();
        }
    }

    public async Task<Dictionary<string, object>> GetTraceAsync(string traceId)
    {
        try
        {
            // This would typically integrate with a distributed tracing system
            // For now, return basic trace information
            return new Dictionary<string, object>
            {
                ["trace_id"] = traceId,
                ["status"] = "completed",
                ["duration_ms"] = 100,
                ["spans"] = new List<object>(),
                ["timestamp"] = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting trace {TraceId}", traceId);
            return new Dictionary<string, object> { ["error"] = ex.Message };
        }
    }

    public async Task StartTraceAsync(string traceName, Dictionary<string, string>? attributes = null)
    {
        try
        {
            var traceId = Guid.NewGuid().ToString();
            _traces[traceId] = DateTime.UtcNow;

            await RecordCustomEventAsync($"Trace Started: {traceName}", "Tracing", new Dictionary<string, object>
            {
                ["trace_id"] = traceId,
                ["trace_name"] = traceName,
                ["attributes"] = attributes ?? new Dictionary<string, string>()
            });

            _logger.LogInformation("Started trace {TraceName} with ID {TraceId}", traceName, traceId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting trace {TraceName}", traceName);
        }
    }

    public async Task EndTraceAsync(string traceId, bool success = true, Dictionary<string, string>? attributes = null)
    {
        try
        {
            if (_traces.TryGetValue(traceId, out var startTime))
            {
                var duration = DateTime.UtcNow - startTime;
                _traces.Remove(traceId);

                await RecordCustomEventAsync($"Trace Ended", "Tracing", new Dictionary<string, object>
                {
                    ["trace_id"] = traceId,
                    ["duration_ms"] = duration.TotalMilliseconds,
                    ["success"] = success,
                    ["attributes"] = attributes ?? new Dictionary<string, string>()
                });

                _logger.LogInformation("Ended trace {TraceId} with duration {Duration}ms", traceId, duration.TotalMilliseconds);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ending trace {TraceId}", traceId);
        }
    }

    public async Task AddSpanAsync(string traceId, string spanName, Dictionary<string, string>? attributes = null)
    {
        try
        {
            await RecordCustomEventAsync($"Span: {spanName}", "Tracing", new Dictionary<string, object>
            {
                ["trace_id"] = traceId,
                ["span_name"] = spanName,
                ["attributes"] = attributes ?? new Dictionary<string, string>()
            });

            _logger.LogInformation("Added span {SpanName} to trace {TraceId}", spanName, traceId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding span {SpanName} to trace {TraceId}", spanName, traceId);
        }
    }

    private double GetCpuUsage()
    {
        try
        {
            var process = Process.GetCurrentProcess();
            var startTime = DateTime.UtcNow;
            var startCpuUsage = process.TotalProcessorTime;
            
            Thread.Sleep(100);
            
            var endTime = DateTime.UtcNow;
            var endCpuUsage = process.TotalProcessorTime;
            
            var cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
            var totalMsPassed = (endTime - startTime).TotalMilliseconds;
            var cpuUsageTotal = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed);
            
            return cpuUsageTotal * 100;
        }
        catch
        {
            return 0.0;
        }
    }

    private double CalculatePercentile(List<double> values, double percentile)
    {
        if (!values.Any()) return 0;
        
        var sortedValues = values.OrderBy(x => x).ToList();
        var index = (int)Math.Ceiling(percentile * sortedValues.Count) - 1;
        return sortedValues[Math.Max(0, Math.Min(index, sortedValues.Count - 1))];
    }
}
