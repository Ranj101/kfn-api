using Microsoft.EntityFrameworkCore;

namespace KfnApi.Models.Common;

public class PaginatedList<T> : List<T>
{
    private PaginatedList(IEnumerable<T> items, int count, int pageIndex, int pageSize)
    {
        TotalCount = count;
        PageSize = pageSize;
        PageIndex = pageIndex;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        AddRange(items);
    }

    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int PageIndex { get; set; }
    public int TotalPages { get; set; }

    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
    {
        var count = await source.CountAsync();
        var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PaginatedList<T>(items, count, pageIndex, pageSize);
    }
}
