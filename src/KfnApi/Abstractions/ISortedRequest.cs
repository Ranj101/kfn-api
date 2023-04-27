using KfnApi.Models.Enums;

namespace KfnApi.Abstractions;

public interface ISortedRequest<TSortBy> where TSortBy : Enum
{
    TSortBy SortBy { get; set; }
    SortDirection SortDirection { get; set; }
}
