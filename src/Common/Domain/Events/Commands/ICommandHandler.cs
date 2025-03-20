namespace Common.Domain.Events.Commands;

public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
where TCommand : ICommand<TResponse>;

public record SomeClass : BaseCommand<SomeClass2>;

public record SomeClass2;

public class Handler : ICommandHandler<SomeClass, Result<SomeClass2>>
{
    public async Task<Result<SomeClass2>> Handle(SomeClass request, CancellationToken cancellationToken)
    {
        var t = new SomeClass2();

        return Result<SomeClass2>.Ok(t);
    }
}