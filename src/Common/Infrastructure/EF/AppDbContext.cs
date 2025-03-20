using App.Server.Notification.Application.Abstractions;

namespace Common.Infrastructure.EF;

public abstract class AppDbContext(DbContextOptions options) : DbContext(options), IUnitOfWork
{
    private const string DefaultLoggingContext = "Transaction";

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }

    /// <inheritdoc />
    public Result UseTransaction(Action action, [CallerMemberName] string loggingContext = DefaultLoggingContext)
    {
        return Database
            .CreateExecutionStrategy()
            .Execute(() =>
            {
                using var transaction = Database.BeginTransaction();

                try
                {
                    action();
                    transaction.Commit();
                    return Result.Ok();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    return Result.Fail(new Error(ex));
                }
            });
    }

    /// <inheritdoc />
    public Result<T> UseTransaction<T>(Func<T> action, [CallerMemberName] string loggingContext = DefaultLoggingContext)
    {
        return Database
            .CreateExecutionStrategy()
            .Execute(() =>
            {
                using var transaction = Database.BeginTransaction();

                try
                {
                    var result = action();
                    transaction.Commit();

                    return Result<T>.Ok(result);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    return Result<T>.Fail(new Error(ex));
                }
            });
    }

    /// <inheritdoc />
    public Result<T> UseTransaction<T>(
        Func<Result<T>> action,
        [CallerMemberName] string loggingContext = DefaultLoggingContext
    )
    {
        return Database
            .CreateExecutionStrategy()
            .Execute(() =>
            {
                using var transaction = Database.BeginTransaction();

                var result = action();
                if (!result.IsSuccess)
                {
                    transaction.Rollback();
                    return result;
                }

                transaction.Commit();
                return result;
            });
    }

    /// <inheritdoc />
    public async Task<Result> UseTransactionAsync(
        Func<Task> action,
        CancellationToken cancellationToken = default,
        [CallerMemberName] string loggingContext = DefaultLoggingContext
    )
    {
        return await Database
            .CreateExecutionStrategy()
            .ExecuteAsync(async () =>
            {
                await using var transaction = await Database.BeginTransactionAsync(cancellationToken);

                try
                {
                    await action();
                    await transaction.CommitAsync(cancellationToken);
                    return Result.Ok();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return Result.Fail(new Error(ex));
                }
            });
    }

    /// <inheritdoc />
    public async Task<Result<T>> UseTransactionAsync<T>(
        Func<Task<T>> action,
        CancellationToken cancellationToken = default,
        [CallerMemberName] string loggingContext = DefaultLoggingContext
    )
    {
        return await Database
            .CreateExecutionStrategy()
            .ExecuteAsync(async () =>
            {
                await using var transaction = await Database.BeginTransactionAsync(cancellationToken);

                try
                {
                    var result = await action();
                    await transaction.CommitAsync(cancellationToken);
                    return Result<T>.Ok(result);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return Result<T>.Fail(new Error(ex));
                }
            });
    }

    /// <inheritdoc />
    public async Task<Result<T>> UseTransaction<T>(
        Func<Task<Result<T>>> action,
        [CallerMemberName] string loggingContext = DefaultLoggingContext
    )
    {
        return await Database
            .CreateExecutionStrategy()
            .ExecuteAsync(async () =>
            {
                await using var transaction = await Database.BeginTransactionAsync();

                var result = await action();
                if (!result.IsSuccess)
                {
                    await transaction.RollbackAsync();
                    return result;
                }

                await transaction.CommitAsync();
                return result;
            });
    }
}
