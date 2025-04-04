namespace Common.Application.Extensions;

// we should not relate to Ef or Mongo here, and we should design as general with IQueryable to work with any providers
public static class QueryableExtensions
{
    // without sorting, we will have performance issue
    public static async Task<IPageList<TEntity>> ApplyPagingAsync<TEntity, TSortKey>(
        this IQueryable<TEntity> collection,
        IPageRequest pageRequest,
        ISieveProcessor sieveProcessor,
        Expression<Func<TEntity, TSortKey>> sortExpression,
        CancellationToken cancellationToken
    )
        where TEntity : class
    {
        IQueryable<TEntity> queryable = collection;
        queryable = queryable.OrderByDescending(sortExpression);

        var sieveModel = new SieveModel
        {
            PageSize = pageRequest.PageSize,
            Page = pageRequest.PageNumber,
            Sorts = pageRequest.SortOrder,
            Filters = pageRequest.Filters,
        };

        var result = sieveProcessor.Apply(sieveModel, queryable, applyPagination: false);

        // Ensure ordering is applied before applying pagination
        if (!result.Expression.ToString().Contains("OrderBy", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("The query must include an 'OrderBy' clause before pagination.");
        }

        // The provider for the source 'IQueryable' doesn't implement 'IAsyncQueryProvider'. Only providers that implement 'IAsyncQueryProvider' can be used for Entity Framework asynchronous operations.
        var total = result.Count();
        result = sieveProcessor.Apply(sieveModel, queryable, applyFiltering: false, applySorting: false);
        var items = await result.AsNoTracking().ToAsyncEnumerable().ToListAsync(cancellationToken: cancellationToken);

        return PageList<TEntity>.Create(items.AsReadOnly(), pageRequest.PageNumber, pageRequest.PageSize, total);
    }

    public static async Task<IPageList<TResult>> ApplyPagingAsync<TEntity, TResult, TSortKey>(
        this IQueryable<TEntity> collection,
        IPageRequest pageRequest,
        ISieveProcessor sieveProcessor,
        Func<IQueryable<TEntity>, IQueryable<TResult>> projectionFunc,
        Expression<Func<TEntity, TSortKey>> sortExpression,
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default
    )
        where TEntity : class
        where TResult : class
    {
        IQueryable<TEntity> queryable = collection;
        if (predicate is not null)
        {
            queryable = queryable.Where(predicate);
        }

        queryable = queryable.OrderByDescending(sortExpression);

        var sieveModel = new SieveModel
        {
            PageSize = pageRequest.PageSize,
            Page = pageRequest.PageNumber,
            Sorts = pageRequest.SortOrder,
            Filters = pageRequest.Filters,
        };

        var result = sieveProcessor.Apply(sieveModel, queryable, applyPagination: false);

        // The provider for the source 'IQueryable' doesn't implement 'IAsyncQueryProvider'. Only providers that implement 'IAsyncQueryProvider' can be used for Entity Framework asynchronous operations.
        var total = result.Count();
        result = sieveProcessor.Apply(sieveModel, queryable, applyFiltering: false, applySorting: true); // Only applies pagination
        var projectedQuery = projectionFunc(result);

        var items = await projectedQuery
            .AsNoTracking()
            .ToAsyncEnumerable()
            .ToListAsync(cancellationToken: cancellationToken);

        return PageList<TResult>.Create(items.AsReadOnly(), pageRequest.PageNumber, pageRequest.PageSize, total);
    }

    public static async Task<IPageList<TResult>> ApplyPagingAsync<TEntity, TResult, TSortKey>(
        this IQueryable<TEntity> collection,
        IPageRequest pageRequest,
        ISieveProcessor sieveProcessor,
        Func<TEntity, TResult> map,
        Expression<Func<TEntity, TSortKey>> sortExpression,
        CancellationToken cancellationToken
    )
        where TEntity : class
        where TResult : class
    {
        var queryable = collection;
        queryable = queryable.OrderByDescending(sortExpression);

        var sieveModel = new SieveModel
        {
            PageSize = pageRequest.PageSize,
            Page = pageRequest.PageNumber,
            Sorts = pageRequest.SortOrder,
            Filters = pageRequest.Filters,
        };

        var result = sieveProcessor.Apply(sieveModel, queryable, applyPagination: false);

        var total = result.Count();
        result = sieveProcessor.Apply(sieveModel, queryable, applyFiltering: false, applySorting: false); // Only applies pagination

        var items = await result
            .Select(x => map(x))
            .AsNoTracking()
            .ToAsyncEnumerable()
            .ToListAsync(cancellationToken: cancellationToken);

        return PageList<TResult>.Create(items.AsReadOnly(), pageRequest.PageNumber, pageRequest.PageSize, total);
    }

    public static async Task<IPageList<TEntity>> ApplyPagingAsync<TEntity, TSortKey>(
        this IQueryable<TEntity> collection,
        IPageRequest pageRequest,
        ISieveProcessor sieveProcessor,
        Expression<Func<TEntity, bool>>? predicate = null,
        Expression<Func<TEntity, TSortKey>>? sortExpression = null,
        CancellationToken cancellationToken = default
    )
        where TEntity : class
    {
        var queryable = predicate != null ? collection.Where(predicate) : collection;

        if (sortExpression != null)
        {
            queryable = queryable.OrderByDescending(sortExpression);
        }

        var sieveModel = new SieveModel
        {
            PageSize = pageRequest.PageSize,
            Page = pageRequest.PageNumber,
            Sorts = pageRequest.SortOrder,
            Filters = pageRequest.Filters,
        };

        var result = sieveProcessor.Apply(sieveModel, queryable, applyPagination: false);
        var total = result.Count();
        result = sieveProcessor.Apply(sieveModel, queryable, applyFiltering: false, applySorting: false);

        var items = await result.AsNoTracking().ToListAsync(cancellationToken: cancellationToken);

        return PageList<TEntity>.Create(items.AsReadOnly(), pageRequest.PageNumber, pageRequest.PageSize, total);
    }
}
