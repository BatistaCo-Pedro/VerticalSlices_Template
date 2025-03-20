namespace Common.Domain.Events.Queries;

public abstract record CachedQuery<TResponse> : BaseQuery<TResponse>, ICachedEvent
{
    public virtual bool InvalidateCache => false;
    public virtual bool BypassCache => false;
    public virtual int? LocalCacheExpirationTimeInMinutes => null;
    public virtual int? CacheExpirationTimeInMinutes => null;

    public abstract string Prefix { get; }
    public abstract string CacheKey { get; }
}
