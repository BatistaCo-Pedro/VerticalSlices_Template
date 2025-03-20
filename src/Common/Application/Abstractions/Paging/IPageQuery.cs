using Common.Domain.Events.Queries;

namespace Common.Application.Abstractions.Paging;

public interface IPageQuery<out TResponse> : IPageRequest, IQuery<TResponse>
    where TResponse : class;
