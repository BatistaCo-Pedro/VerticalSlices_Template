namespace Common.Application.Caching;

public class CacheService(HybridCache hybridCache, IOptions<CacheOptions> cacheOptions) : ICacheService
{
    private readonly CacheOptions _cacheOptions = cacheOptions.Value;

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        return await hybridCache.GetOrCreateAsync<T>(
            key,
            _ => ValueTask.FromResult<T?>(default)!,
            cancellationToken: cancellationToken
        );
    }

    public async Task<T> GetOrCreateAsync<T>(
        string key,
        Func<CancellationToken, ValueTask<T>> factory,
        IEnumerable<string>? tags = null,
        int? localExpirationInMinutes = null,
        int? distributedExpirationInMinutes = null,
        CancellationToken cancellationToken = default
    )
    {
        // Use provided expirations or fallback to default from CacheOptions
        var options = new HybridCacheEntryOptions
        {
            LocalCacheExpiration = TimeSpan.FromMinutes(
                localExpirationInMinutes ?? _cacheOptions.LocalCacheExpirationTimeInMinute
            ),
            Expiration = TimeSpan.FromMinutes(distributedExpirationInMinutes ?? _cacheOptions.ExpirationTimeInMinute),
        };

        return await hybridCache.GetOrCreateAsync(
            key: key,
            factory: factory,
            options: options,
            tags: tags,
            cancellationToken: cancellationToken
        );
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        IEnumerable<string>? tags = null,
        int? localExpirationInMinutes = null,
        int? distributedExpirationInMinutes = null,
        CancellationToken cancellationToken = default
    )
    {
        // Use provided expirations or fallback to default from CacheOptions
        var options = new HybridCacheEntryOptions
        {
            LocalCacheExpiration = TimeSpan.FromMinutes(
                localExpirationInMinutes ?? _cacheOptions.LocalCacheExpirationTimeInMinute
            ),
            Expiration = TimeSpan.FromMinutes(distributedExpirationInMinutes ?? _cacheOptions.ExpirationTimeInMinute),
        };

        await hybridCache.SetAsync(
            key: key,
            value: value,
            options: options,
            tags: tags,
            cancellationToken: cancellationToken
        );
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        var value = await hybridCache.GetOrCreateAsync<object?>(
            key,
            _ => ValueTask.FromResult<object?>(null),
            cancellationToken: cancellationToken
        );

        return value != null;
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await hybridCache.RemoveAsync(key, cancellationToken);
    }

    public async Task RemoveByTagsAsync(IEnumerable<string> tags, CancellationToken cancellationToken = default)
    {
        await hybridCache.RemoveByTagAsync(tags, cancellationToken);
    }
}
