namespace KfnApi.Abstractions;

public interface IPaginatedResponse
{
    int Page { get; set; }
    int Count { get; set; }
    int PageSize { get; set; }
    int TotalPages { get; set; }
}
