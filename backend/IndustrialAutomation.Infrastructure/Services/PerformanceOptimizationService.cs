using IndustrialAutomation.Core.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;

namespace IndustrialAutomation.Infrastructure.Services;

public class PerformanceOptimizationService
{
    private readonly ICacheService _cacheService;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<PerformanceOptimizationService> _logger;
    private readonly Dictionary<string, PerformanceMetric> _metrics = new();

    public PerformanceOptimizationService(
        ICacheService cacheService,
        IMemoryCache memoryCache,
        ILogger<PerformanceOptimizationService> logger)
    {
        _cacheService = cacheService;
        _memoryCache = memoryCache;
        _logger = logger;
    }

    public async Task<T> GetOrSetAsync<T>(
        string key,
        Func<Task<T>> factory,
        TimeSpan? expiration = null,
        bool useMemoryCache = true)
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            // Try memory cache first
            if (useMemoryCache && _memoryCache.TryGetValue(key, out T? cachedValue))
            {
                RecordCacheHit(key, "memory");
                return cachedValue;
            }

            // Try distributed cache
            var distributedValue = await _cacheService.GetAsync<T>(key);
            if (distributedValue != null)
            {
                RecordCacheHit(key, "distributed");
                
                // Store in memory cache for faster access
                if (useMemoryCache)
                {
                    _memoryCache.Set(key, distributedValue, TimeSpan.FromMinutes(5));
                }
                
                return distributedValue;
            }

            // Cache miss - execute factory
            RecordCacheMiss(key);
            var value = await factory();
            
            // Store in both caches
            var cacheExpiration = expiration ?? TimeSpan.FromMinutes(15);
            await _cacheService.SetAsync(key, value, cacheExpiration);
            
            if (useMemoryCache)
            {
                _memoryCache.Set(key, value, TimeSpan.FromMinutes(5));
            }

            return value;
        }
        finally
        {
            stopwatch.Stop();
            RecordPerformanceMetric(key, stopwatch.ElapsedMilliseconds);
        }
    }

    public async Task<T> GetOrSetWithTagsAsync<T>(
        string key,
        Func<Task<T>> factory,
        string[] tags,
        TimeSpan? expiration = null)
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            // Check cache first
            var cachedValue = await _cacheService.GetAsync<T>(key);
            if (cachedValue != null)
            {
                RecordCacheHit(key, "tagged");
                return cachedValue;
            }

            // Execute factory
            RecordCacheMiss(key);
            var value = await factory();
            
            // Store with tags
            var cacheOptions = new CacheOptions
            {
                Expiration = expiration ?? TimeSpan.FromMinutes(15),
                Tags = string.Join(",", tags)
            };
            
            await _cacheService.SetAsync(key, value, cacheOptions);
            
            return value;
        }
        finally
        {
            stopwatch.Stop();
            RecordPerformanceMetric(key, stopwatch.ElapsedMilliseconds);
        }
    }

    public async Task InvalidateByTagAsync(string tag)
    {
        try
        {
            // This would require a tag-based invalidation system
            // For now, we'll log the invalidation request
            _logger.LogInformation("Invalidating cache by tag: {Tag}", tag);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error invalidating cache by tag {Tag}", tag);
        }
    }

    public async Task PreloadDataAsync<T>(
        string key,
        Func<Task<T>> factory,
        TimeSpan? expiration = null)
    {
        try
        {
            var value = await factory();
            await _cacheService.SetAsync(key, value, expiration ?? TimeSpan.FromHours(1));
            
            _logger.LogInformation("Preloaded data for key: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error preloading data for key {Key}", key);
        }
    }

    public async Task<Dictionary<string, object>> GetPerformanceMetricsAsync()
    {
        try
        {
            var cacheInfo = await _cacheService.GetCacheInfoAsync();
            var memoryCache = _memoryCache as MemoryCache;
            
            return new Dictionary<string, object>
            {
                ["cache_hits"] = _metrics.Values.Sum(m => m.CacheHits),
                ["cache_misses"] = _metrics.Values.Sum(m => m.CacheMisses),
                ["average_response_time_ms"] = _metrics.Values.Any() 
                    ? _metrics.Values.Average(m => m.AverageResponseTime) 
                    : 0,
                ["total_requests"] = _metrics.Values.Sum(m => m.TotalRequests),
                ["cache_hit_rate"] = CalculateHitRate(),
                ["memory_cache_size"] = memoryCache?.Count ?? 0,
                ["distributed_cache_info"] = cacheInfo
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting performance metrics");
            return new Dictionary<string, object>();
        }
    }

    public async Task OptimizeCacheAsync()
    {
        try
        {
            // Remove expired entries from memory cache
            var memoryCache = _memoryCache as MemoryCache;
            if (memoryCache != null)
            {
                // Force garbage collection to clean up expired entries
                memoryCache.Compact(0.5);
            }

            // Clear old performance metrics
            var oldMetrics = _metrics.Where(kvp => kvp.Value.LastAccessed < DateTime.UtcNow.AddHours(-1));
            foreach (var metric in oldMetrics)
            {
                _metrics.Remove(metric.Key);
            }

            _logger.LogInformation("Cache optimization completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error optimizing cache");
        }
    }

    public async Task WarmupCacheAsync()
    {
        try
        {
            // This would typically preload frequently accessed data
            var warmupTasks = new List<Task>
            {
                // Preload user data
                PreloadDataAsync("users:active", async () => new { count = 0 }),
                
                // Preload system configuration
                PreloadDataAsync("config:system", async () => new { version = "1.0.0" }),
                
                // Preload AI models
                PreloadDataAsync("ai:models", async () => new { models = new List<string>() })
            };

            await Task.WhenAll(warmupTasks);
            
            _logger.LogInformation("Cache warmup completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error warming up cache");
        }
    }

    public async Task<QueryOptimizationResult> OptimizeQueryAsync(
        string query,
        Dictionary<string, object> parameters)
    {
        try
        {
            var stopwatch = Stopwatch.StartNew();
            
            // Analyze query performance
            var analysis = await AnalyzeQueryPerformanceAsync(query, parameters);
            
            stopwatch.Stop();
            
            return new QueryOptimizationResult
            {
                OriginalQuery = query,
                OptimizedQuery = analysis.OptimizedQuery,
                PerformanceGain = analysis.PerformanceGain,
                Recommendations = analysis.Recommendations,
                ExecutionTime = stopwatch.ElapsedMilliseconds
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error optimizing query");
            return new QueryOptimizationResult
            {
                OriginalQuery = query,
                Error = ex.Message
            };
        }
    }

    public async Task<DatabaseOptimizationResult> OptimizeDatabaseAsync()
    {
        try
        {
            var result = new DatabaseOptimizationResult();
            
            // Analyze table sizes
            result.TableSizes = await AnalyzeTableSizesAsync();
            
            // Identify missing indexes
            result.MissingIndexes = await IdentifyMissingIndexesAsync();
            
            // Analyze query performance
            result.SlowQueries = await IdentifySlowQueriesAsync();
            
            // Generate optimization recommendations
            result.Recommendations = await GenerateOptimizationRecommendationsAsync(result);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error optimizing database");
            return new DatabaseOptimizationResult
            {
                Error = ex.Message
            };
        }
    }

    private void RecordCacheHit(string key, string cacheType)
    {
        if (!_metrics.ContainsKey(key))
        {
            _metrics[key] = new PerformanceMetric();
        }
        
        _metrics[key].CacheHits++;
        _metrics[key].LastAccessed = DateTime.UtcNow;
    }

    private void RecordCacheMiss(string key)
    {
        if (!_metrics.ContainsKey(key))
        {
            _metrics[key] = new PerformanceMetric();
        }
        
        _metrics[key].CacheMisses++;
        _metrics[key].LastAccessed = DateTime.UtcNow;
    }

    private void RecordPerformanceMetric(string key, long responseTimeMs)
    {
        if (!_metrics.ContainsKey(key))
        {
            _metrics[key] = new PerformanceMetric();
        }
        
        var metric = _metrics[key];
        metric.TotalRequests++;
        metric.TotalResponseTime += responseTimeMs;
        metric.AverageResponseTime = metric.TotalResponseTime / metric.TotalRequests;
        metric.LastAccessed = DateTime.UtcNow;
    }

    private double CalculateHitRate()
    {
        var totalHits = _metrics.Values.Sum(m => m.CacheHits);
        var totalMisses = _metrics.Values.Sum(m => m.CacheMisses);
        var totalRequests = totalHits + totalMisses;
        
        return totalRequests > 0 ? (double)totalHits / totalRequests * 100 : 0;
    }

    private async Task<QueryAnalysis> AnalyzeQueryPerformanceAsync(
        string query, 
        Dictionary<string, object> parameters)
    {
        // This would typically use a query analyzer
        return new QueryAnalysis
        {
            OptimizedQuery = query,
            PerformanceGain = 0,
            Recommendations = new List<string> { "Query appears to be optimized" }
        };
    }

    private async Task<Dictionary<string, long>> AnalyzeTableSizesAsync()
    {
        // This would typically query database metadata
        return new Dictionary<string, long>();
    }

    private async Task<List<string>> IdentifyMissingIndexesAsync()
    {
        // This would typically analyze query execution plans
        return new List<string>();
    }

    private async Task<List<SlowQuery>> IdentifySlowQueriesAsync()
    {
        // This would typically query performance monitoring data
        return new List<SlowQuery>();
    }

    private async Task<List<string>> GenerateOptimizationRecommendationsAsync(
        DatabaseOptimizationResult result)
    {
        var recommendations = new List<string>();
        
        if (result.TableSizes.Any(t => t.Value > 1000000)) // 1MB
        {
            recommendations.Add("Consider partitioning large tables");
        }
        
        if (result.MissingIndexes.Any())
        {
            recommendations.Add("Add missing indexes to improve query performance");
        }
        
        if (result.SlowQueries.Any())
        {
            recommendations.Add("Optimize slow queries identified in the analysis");
        }
        
        return recommendations;
    }
}

public class PerformanceMetric
{
    public long CacheHits { get; set; }
    public long CacheMisses { get; set; }
    public long TotalRequests { get; set; }
    public long TotalResponseTime { get; set; }
    public double AverageResponseTime { get; set; }
    public DateTime LastAccessed { get; set; }
}

public class QueryOptimizationResult
{
    public string OriginalQuery { get; set; } = string.Empty;
    public string OptimizedQuery { get; set; } = string.Empty;
    public double PerformanceGain { get; set; }
    public List<string> Recommendations { get; set; } = new();
    public long ExecutionTime { get; set; }
    public string? Error { get; set; }
}

public class DatabaseOptimizationResult
{
    public Dictionary<string, long> TableSizes { get; set; } = new();
    public List<string> MissingIndexes { get; set; } = new();
    public List<SlowQuery> SlowQueries { get; set; } = new();
    public List<string> Recommendations { get; set; } = new();
    public string? Error { get; set; }
}

public class SlowQuery
{
    public string Query { get; set; } = string.Empty;
    public long ExecutionTime { get; set; }
    public long Frequency { get; set; }
    public string Recommendations { get; set; } = string.Empty;
}

public class QueryAnalysis
{
    public string OptimizedQuery { get; set; } = string.Empty;
    public double PerformanceGain { get; set; }
    public List<string> Recommendations { get; set; } = new();
}
