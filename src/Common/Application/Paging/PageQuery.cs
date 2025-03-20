namespace Common.Application.Paging;

public record PageQuery<TResponse> : PageRequest, IPageQuery<TResponse>
    where TResponse : class;
