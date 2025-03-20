namespace Common.Infrastructure.EF.Interceptors;

public class AuditInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default
    )
    {
        if (eventData.Context == null)
            return base.SavingChangesAsync(eventData, result, cancellationToken);

        var now = DateTime.Now;

        foreach (var entry in eventData.Context.ChangeTracker.Entries<IAuditable>())
        {
            switch (entry.State)
            {
                case EntityState.Modified:
                    entry.CurrentValues[nameof(IAuditable.LastModified)] = now;
                    entry.CurrentValues[nameof(IAuditable.LastModifiedBy)] = 1;
                    break;
                case EntityState.Added:
                    entry.CurrentValues[nameof(IAuditable.Created)] = now;
                    entry.CurrentValues[nameof(IAuditable.CreatedBy)] = 1;
                    break;
            }
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
