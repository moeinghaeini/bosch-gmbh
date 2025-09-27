using IndustrialAutomation.Core.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace IndustrialAutomation.Infrastructure.Services;

public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<RedisCacheService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public RedisCacheService(IDistributedCache cache, ILogger<RedisCacheService> logger)
    {
        _cache = cache;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        try
        {
            var value = await _cache.GetStringAsync(key);
            if (string.IsNullOrEmpty(value))
                return default;

            return JsonSerializer.Deserialize<T>(value, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cache key {Key}", key);
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        try
        {
            var options = new DistributedCacheEntryOptions();
            
            if (expiration.HasValue)
            {
                options.AbsoluteExpirationRelativeToNow = expiration.Value;
            }

            var serializedValue = JsonSerializer.Serialize(value, _jsonOptions);
            await _cache.SetStringAsync(key, serializedValue, options);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting cache key {Key}", key);
        }
    }

    public async Task SetAsync<T>(string key, T value, CacheOptions options)
    {
        try
        {
            var cacheOptions = new DistributedCacheEntryOptions();
            
            if (options.Expiration.HasValue)
            {
                if (options.SlidingExpiration)
                {
                    cacheOptions.SlidingExpiration = options.Expiration.Value;
                }
                else
                {
                    cacheOptions.AbsoluteExpirationRelativeToNow = options.Expiration.Value;
                }
            }

            var serializedValue = JsonSerializer.Serialize(value, _jsonOptions);
            await _cache.SetStringAsync(key, serializedValue, cacheOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting cache key {Key} with options", key);
        }
    }

    public async Task<bool> RemoveAsync(string key)
    {
        try
        {
            await _cache.RemoveAsync(key);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cache key {Key}", key);
            return false;
        }
    }

    public async Task<bool> ExistsAsync(string key)
    {
        try
        {
            var value = await _cache.GetStringAsync(key);
            return !string.IsNullOrEmpty(value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking cache key {Key}", key);
            return false;
        }
    }

    public async Task<long> IncrementAsync(string key, long value = 1)
    {
        try
        {
            var currentValue = await GetAsync<long>(key);
            var newValue = currentValue + value;
            await SetAsync(key, newValue);
            return newValue;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error incrementing cache key {Key}", key);
            return 0;
        }
    }

    public async Task<long> DecrementAsync(string key, long value = 1)
    {
        try
        {
            var currentValue = await GetAsync<long>(key);
            var newValue = Math.Max(0, currentValue - value);
            await SetAsync(key, newValue);
            return newValue;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error decrementing cache key {Key}", key);
            return 0;
        }
    }

    public async Task<bool> ExpireAsync(string key, TimeSpan expiration)
    {
        try
        {
            var value = await _cache.GetStringAsync(key);
            if (string.IsNullOrEmpty(value))
                return false;

            await SetAsync(key, value, expiration);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting expiration for cache key {Key}", key);
            return false;
        }
    }

    public async Task<TimeSpan?> GetExpirationAsync(string key)
    {
        try
        {
            // Redis doesn't provide TTL information through IDistributedCache
            // This would require direct Redis connection
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting expiration for cache key {Key}", key);
            return null;
        }
    }

    public async Task<Dictionary<string, T>> GetManyAsync<T>(IEnumerable<string> keys)
    {
        var result = new Dictionary<string, T>();
        
        try
        {
            var tasks = keys.Select(async key =>
            {
                var value = await GetAsync<T>(key);
                return new { Key = key, Value = value };
            });

            var results = await Task.WhenAll(tasks);
            
            foreach (var item in results)
            {
                if (item.Value != null)
                {
                    result[item.Key] = item.Value;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting multiple cache keys");
        }

        return result;
    }

    public async Task SetManyAsync<T>(Dictionary<string, T> values, TimeSpan? expiration = null)
    {
        try
        {
            var tasks = values.Select(async kvp =>
            {
                await SetAsync(kvp.Key, kvp.Value, expiration);
            });

            await Task.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting multiple cache keys");
        }
    }

    public async Task<bool> RemoveManyAsync(IEnumerable<string> keys)
    {
        try
        {
            var tasks = keys.Select(key => RemoveAsync(key));
            var results = await Task.WhenAll(tasks);
            return results.All(r => r);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing multiple cache keys");
            return false;
        }
    }

    public async Task<List<string>> GetKeysAsync(string pattern)
    {
        try
        {
            // This would require direct Redis connection
            // IDistributedCache doesn't support key pattern matching
            return new List<string>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting keys with pattern {Pattern}", pattern);
            return new List<string>();
        }
    }

    public async Task<long> GetDatabaseSizeAsync()
    {
        try
        {
            // This would require direct Redis connection
            return 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting database size");
            return 0;
        }
    }

    public async Task ClearDatabaseAsync()
    {
        try
        {
            // This would require direct Redis connection
            // IDistributedCache doesn't support database clearing
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing database");
        }
    }

    public async Task<CacheInfo> GetCacheInfoAsync()
    {
        try
        {
            return new CacheInfo
            {
                TotalKeys = 0,
                UsedMemory = 0,
                MaxMemory = 0,
                HitRate = 0,
                TotalHits = 0,
                TotalMisses = 0
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cache info");
            return new CacheInfo();
        }
    }

    public async Task<bool> PingAsync()
    {
        try
        {
            await _cache.SetStringAsync("ping", "pong", new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(1)
            });
            
            var result = await _cache.GetStringAsync("ping");
            return result == "pong";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error pinging cache");
            return false;
        }
    }
}
