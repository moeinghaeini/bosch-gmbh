namespace IndustrialAutomation.Core.Interfaces;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
    Task SetAsync<T>(string key, T value, CacheOptions options);
    Task<bool> RemoveAsync(string key);
    Task<bool> ExistsAsync(string key);
    Task<long> IncrementAsync(string key, long value = 1);
    Task<long> DecrementAsync(string key, long value = 1);
    Task<bool> ExpireAsync(string key, TimeSpan expiration);
    Task<TimeSpan?> GetExpirationAsync(string key);
    Task<Dictionary<string, T>> GetManyAsync<T>(IEnumerable<string> keys);
    Task SetManyAsync<T>(Dictionary<string, T> values, TimeSpan? expiration = null);
    Task<bool> RemoveManyAsync(IEnumerable<string> keys);
    Task<List<string>> GetKeysAsync(string pattern);
    Task<long> GetDatabaseSizeAsync();
    Task ClearDatabaseAsync();
    Task<CacheInfo> GetCacheInfoAsync();
    Task<bool> PingAsync();
}

public class CacheOptions
{
    public TimeSpan? Expiration { get; set; }
    public bool SlidingExpiration { get; set; } = false;
    public CachePriority Priority { get; set; } = CachePriority.Normal;
    public bool Compress { get; set; } = false;
    public string? Tags { get; set; }
    public bool Overwrite { get; set; } = true;
}

public class CacheInfo
{
    public long TotalKeys { get; set; }
    public long UsedMemory { get; set; }
    public long MaxMemory { get; set; }
    public double HitRate { get; set; }
    public long TotalHits { get; set; }
    public long TotalMisses { get; set; }
    public Dictionary<string, long> KeyCountByType { get; set; } = new();
    public List<string> TopKeys { get; set; } = new();
}

public enum CachePriority
{
    Low,
    Normal,
    High
}
