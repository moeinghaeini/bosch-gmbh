using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace IndustrialAutomation.API.Gateway;

public interface IApiGatewayService
{
    Task<T?> RouteRequestAsync<T>(string service, string endpoint, HttpMethod method, object? data = null);
    Task<HealthStatus> GetServiceHealthAsync(string service);
    Task<List<ServiceInfo>> GetAvailableServicesAsync();
}

public class ApiGatewayService : IApiGatewayService
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private readonly ILogger<ApiGatewayService> _logger;
    private readonly Dictionary<string, string> _serviceEndpoints;

    public ApiGatewayService(HttpClient httpClient, IMemoryCache cache, ILogger<ApiGatewayService> logger)
    {
        _httpClient = httpClient;
        _cache = cache;
        _logger = logger;
        _serviceEndpoints = new Dictionary<string, string>
        {
            { "auth", "http://localhost:5002" },
            { "automation", "http://localhost:5003" },
            { "ai", "http://localhost:5004" },
            { "analytics", "http://localhost:5005" },
            { "notifications", "http://localhost:5006" }
        };
    }

    public async Task<T?> RouteRequestAsync<T>(string service, string endpoint, HttpMethod method, object? data = null)
    {
        try
        {
            if (!_serviceEndpoints.ContainsKey(service))
            {
                throw new ArgumentException($"Service '{service}' not found");
            }

            var baseUrl = _serviceEndpoints[service];
            var url = $"{baseUrl}{endpoint}";
            
            var request = new HttpRequestMessage(method, url);
            
            if (data != null)
            {
                var json = JsonSerializer.Serialize(data);
                request.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            }

            var response = await _httpClient.SendAsync(request);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(content);
            }
            
            _logger.LogWarning("Service {Service} returned {StatusCode} for {Endpoint}", 
                service, response.StatusCode, endpoint);
            return default;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error routing request to service {Service}", service);
            return default;
        }
    }

    public async Task<HealthStatus> GetServiceHealthAsync(string service)
    {
        try
        {
            if (!_serviceEndpoints.ContainsKey(service))
            {
                return new HealthStatus { Service = service, IsHealthy = false, Error = "Service not found" };
            }

            var baseUrl = _serviceEndpoints[service];
            var response = await _httpClient.GetAsync($"{baseUrl}/health");
            
            return new HealthStatus
            {
                Service = service,
                IsHealthy = response.IsSuccessStatusCode,
                ResponseTime = response.Headers.GetValues("X-Response-Time").FirstOrDefault(),
                Error = response.IsSuccessStatusCode ? null : $"HTTP {response.StatusCode}"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking health for service {Service}", service);
            return new HealthStatus { Service = service, IsHealthy = false, Error = ex.Message };
        }
    }

    public async Task<List<ServiceInfo>> GetAvailableServicesAsync()
    {
        var services = new List<ServiceInfo>();
        
        foreach (var service in _serviceEndpoints.Keys)
        {
            var health = await GetServiceHealthAsync(service);
            services.Add(new ServiceInfo
            {
                Name = service,
                Endpoint = _serviceEndpoints[service],
                IsHealthy = health.IsHealthy,
                LastChecked = DateTime.UtcNow
            });
        }
        
        return services;
    }
}

public class HealthStatus
{
    public string Service { get; set; } = string.Empty;
    public bool IsHealthy { get; set; }
    public string? ResponseTime { get; set; }
    public string? Error { get; set; }
}

public class ServiceInfo
{
    public string Name { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public bool IsHealthy { get; set; }
    public DateTime LastChecked { get; set; }
}
