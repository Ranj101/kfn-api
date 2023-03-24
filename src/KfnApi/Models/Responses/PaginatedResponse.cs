using KfnApi.Abstractions;
using KfnApi.Models.Common;

namespace KfnApi.Models.Responses;

public sealed record PaginatedResponse<T, TMapped> : IPaginatedResponse
{
    public int Page { get; set; }
    public int Count { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public List<TMapped> Data { get; set; }

    public PaginatedResponse(PaginatedList<T> paginated, List<TMapped> data)
    {
        Page = paginated.PageIndex;
        Count = paginated.TotalCount;
        PageSize = paginated.PageSize;
        TotalPages = paginated.TotalPages;
        Data = data;
    }
}
