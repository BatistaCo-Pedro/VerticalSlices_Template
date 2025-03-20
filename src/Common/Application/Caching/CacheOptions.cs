namespace Common.Application.Caching;

public class CacheOptions
{
    public double ExpirationTimeInMinute { get; set; } = 30;
    public double LocalCacheExpirationTimeInMinute { get; set; } = 5;
    public required RedisDistributedCacheOptions RedisCacheOptions { get; set; }
    public string DefaultCachePrefix { get; set; } = "Ch_";
}

public class RedisDistributedCacheOptions
{
    public required string Host { get; set; }
    public int Port { get; set; }
    public bool AllowAdmin { get; set; }
}
