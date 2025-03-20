namespace Common.Infrastructure.Behaviors;

public class CachingBehavior<TRequest, TResponse>(ICacheService cacheService) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICachedEvent
    where TResponse : Result
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        TResponse? cachedResponse = null;
        if (request.InvalidateCache)
        {
            await cacheService.RemoveAsync(request.CacheKey, cancellationToken);
            Log.Debug("Cache data with cache key: {CacheKey} invalidated", request.CacheKey);
        }
        else
        {
            cachedResponse = await cacheService.GetAsync<TResponse>(request.CacheKey, cancellationToken);
        }

        if (request.BypassCache)
        {
            return await next();
        }


        if (cachedResponse != null)
        {
            Log.Debug(
                "Response retrieved {TRequest} from cache. CacheKey: {CacheKey}",
                typeof(TRequest).FullName,
                request.CacheKey
            );

            return cachedResponse;
        }

        var response = await next();

        await cacheService.SetAsync(
            key: request.CacheKey,
            value: response,
            localExpirationInMinutes: request.LocalCacheExpirationTimeInMinutes,
            distributedExpirationInMinutes: request.CacheExpirationTimeInMinutes,
            cancellationToken: cancellationToken
        );

        Log.Debug(
            "Caching response for {TRequest} with cache key: {CacheKey}",
            typeof(TRequest).FullName,
            request.CacheKey
        );

        return response;
    }
}
