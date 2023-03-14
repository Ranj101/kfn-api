using KfnApi.Models.Enums;

namespace KfnApi.Abstractions;

public interface ISortedRequest
{
    SortBy SortBy { get; set; }
    SortDirection SortDirection { get; set; }
}
