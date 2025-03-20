namespace Common.Infrastructure.EF.Interceptors;

public class DomainEventInterceptor(IDomainEventPublisher domainEventPublisher) : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default
    )
    {
        if (eventData.Context is not AppDbContext dbContext)
            return await base.SavingChangesAsync(eventData, result, cancellationToken);

        var entities = dbContext
            .ChangeTracker.Entries<IEntity>()
            .Select(x => x.Entity)
            .Where(x => !x.DomainEvents.IsEmpty)
            .ToList();

        await domainEventPublisher.PublishAsync(entities, cancellationToken).ConfigureAwait(true);

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
