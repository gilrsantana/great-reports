using GreatReports.Shared;

namespace GreatReports.Application.Common.CQRS;

public interface ICommand
{
}

public interface ICommand<TResponse>
{
}

public interface IQuery<TResponse>
{
}

public interface ICommandHandler<TCommand>
    where TCommand : ICommand
{
    Task<Result> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}

public interface ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    Task<Result<TResponse>> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}

public interface IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    Task<Result<TResponse>> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}
