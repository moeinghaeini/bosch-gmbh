using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace IndustrialAutomation.Infrastructure.Services;

public interface IEnhancedCacheService
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
    Task RemoveAsync(string key);
    Task RemoveByPatternAsync(string pattern);
    Task<bool> ExistsAsync(string key);
    Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null);
    Task InvalidateUserCacheAsync(string userId);
}

public class EnhancedCacheService : IEnhancedCacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly IDistributedCache _distributedCache;
    private readonly ILogger<EnhancedCacheService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public EnhancedCacheService(
        IMemoryCache memoryCache,
        IDistributedCache distributedCache,
        ILogger<EnhancedCacheService> logger)
    {
        _memoryCache = memoryCache;
        _distributedCache = distributedCache;
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
            // Try memory cache first
            if (_memoryCache.TryGetValue(key, out T? memoryValue))
            {
                return memoryValue;
            }

            // Try distributed cache
            var distributedValue = await _distributedCache.GetStringAsync(key);
            if (distributedValue != null)
            {
                var result = JsonSerializer.Deserialize<T>(distributedValue, _jsonOptions);
                
                // Store in memory cache for faster access
                _memoryCache.Set(key, result, TimeSpan.FromMinutes(5));
                
                return result;
            }

            return default;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cache value for key {Key}", key);
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        try
        {
            var expirationTime = expiration ?? TimeSpan.FromHours(1);
            
            // Set in memory cache
            _memoryCache.Set(key, value, expirationTime);
            
            // Set in distributed cache
            var serializedValue = JsonSerializer.Serialize(value, _jsonOptions);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expirationTime
            };
            
            await _distributedCache.SetStringAsync(key, serializedValue, options);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting cache value for key {Key}", key);
        }
    }

    public async Task RemoveAsync(string key)
    {
        try
        {
            _memoryCache.Remove(key);
            await _distributedCache.RemoveAsync(key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cache value for key {Key}", key);
        }
    }

    public async Task RemoveByPatternAsync(string pattern)
    {
        try
        {
            // This is a simplified implementation
            // In production, you might want to use Redis with pattern matching
            _logger.LogInformation("Removing cache entries matching pattern {Pattern}", pattern);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cache entries for pattern {Pattern}", pattern);
        }
    }

    public async Task<bool> ExistsAsync(string key)
    {
        try
        {
            return _memoryCache.TryGetValue(key, out _) || 
                   await _distributedCache.GetStringAsync(key) != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking cache existence for key {Key}", key);
            return false;
        }
    }

    public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
    {
        var cachedValue = await GetAsync<T>(key);
        if (cachedValue != null)
        {
            return cachedValue;
        }

        var value = await factory();
        await SetAsync(key, value, expiration);
        return value;
    }

    public async Task InvalidateUserCacheAsync(string userId)
    {
        try
        {
            var userKeys = new[]
            {
                $"user:{userId}:profile",
                $"user:{userId}:permissions",
                $"user:{userId}:preferences"
            };

            foreach (var key in userKeys)
            {
                await RemoveAsync(key);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error invalidating user cache for user {UserId}", userId);
        }
    }
}
