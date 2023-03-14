using KfnApi.Abstractions;

namespace KfnApi.Helpers.Extensions;

public static class SortingExtensions
{
    public static IOrderedQueryable<T> SortBy<T>(this IQueryable<T> source, ISortBy sortBy)
    {
        return Queryable.OrderBy(source, sortBy.Expression);
    }

    public static IOrderedQueryable<T> SortByDescending<T>(this IQueryable<T> source, ISortBy sortBy)
    {
        return Queryable.OrderByDescending(source, sortBy.Expression);
    }

    public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, ISortBy sortBy)
    {
        return Queryable.ThenBy(source, sortBy.Expression);
    }

    public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> source, ISortBy sortBy)
    {
        return Queryable.ThenByDescending(source, sortBy.Expression);
    }
}
