namespace Common.Application.Paging;

public record PageRequest : IPageRequest
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? Filters { get; init; }
    public string? SortOrder { get; init; }

    public void Deconstruct(out int pageNumber, out int pageSize, out string? filters, out string? sortOrder) =>
        (pageNumber, pageSize, filters, sortOrder) = (PageNumber, PageSize, Filters, SortOrder);
}
