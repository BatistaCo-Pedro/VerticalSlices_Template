namespace Common.Domain.Events.Commands;

public abstract record CachedCommand<TResponse> : BaseCommand<TResponse>, ICachedEvent
{
    public virtual bool InvalidateCache => false;

    public virtual bool BypassCache => false;
    public virtual int? LocalCacheExpirationTimeInMinutes => null;
    public virtual int? CacheExpirationTimeInMinutes => null;
    public abstract string Prefix { get; }
    public abstract string CacheKey { get; }
}
