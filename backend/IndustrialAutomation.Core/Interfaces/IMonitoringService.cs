namespace IndustrialAutomation.Core.Interfaces;

public interface IMonitoringService
{
    Task RecordMetricAsync(string metricName, double value, Dictionary<string, string>? tags = null);
    Task RecordCounterAsync(string counterName, int increment = 1, Dictionary<string, string>? tags = null);
    Task RecordTimerAsync(string timerName, TimeSpan duration, Dictionary<string, string>? tags = null);
    Task RecordGaugeAsync(string gaugeName, double value, Dictionary<string, string>? tags = null);
    Task RecordHistogramAsync(string histogramName, double value, Dictionary<string, string>? tags = null);
    
    Task<Dictionary<string, object>> GetSystemHealthAsync();
    Task<Dictionary<string, object>> GetPerformanceMetricsAsync();
    Task<Dictionary<string, object>> GetBusinessMetricsAsync();
    Task<List<Dictionary<string, object>>> GetAlertsAsync();
    
    Task CreateAlertAsync(string alertName, string description, string severity, Dictionary<string, string>? conditions = null);
    Task UpdateAlertAsync(string alertName, Dictionary<string, object> updates);
    Task DeleteAlertAsync(string alertName);
    
    Task RecordEventAsync(string eventName, Dictionary<string, object>? properties = null);
    Task RecordErrorAsync(string errorMessage, Exception? exception = null, Dictionary<string, string>? context = null);
    Task RecordCustomEventAsync(string eventName, string category, Dictionary<string, object>? properties = null);
    
    Task<Dictionary<string, object>> GetDashboardDataAsync();
    Task<List<Dictionary<string, object>>> GetLogsAsync(int page = 1, int pageSize = 50, string? level = null, DateTime? from = null, DateTime? to = null);
    Task<Dictionary<string, object>> GetTraceAsync(string traceId);
    
    Task StartTraceAsync(string traceName, Dictionary<string, string>? attributes = null);
    Task EndTraceAsync(string traceId, bool success = true, Dictionary<string, string>? attributes = null);
    Task AddSpanAsync(string traceId, string spanName, Dictionary<string, string>? attributes = null);
}
