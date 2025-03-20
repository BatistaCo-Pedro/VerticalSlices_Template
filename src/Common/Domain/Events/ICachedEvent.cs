namespace Common.Domain.Events;

public interface ICachedEvent
{
    bool InvalidateCache { get; }
    
    bool BypassCache { get; }
    
    int? LocalCacheExpirationTimeInMinutes { get; }
    int? CacheExpirationTimeInMinutes { get; }

    // TimeSpan SlidingExpiration { get; }
    // DateTime? AbsoluteExpiration { get; }
    string Prefix { get; }
    string CacheKey { get; }
}
