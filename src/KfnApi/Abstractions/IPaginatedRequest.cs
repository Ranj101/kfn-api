namespace KfnApi.Abstractions;

public interface IPaginatedRequest
{
    int PageIndex { get; set; }
    int PageSize { get; set; }
}
