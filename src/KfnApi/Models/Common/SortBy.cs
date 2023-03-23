using System.Linq.Expressions;
using KfnApi.Abstractions;

namespace KfnApi.Models.Common;

public class SortBy<TItem, T> : ISortBy
{
    private readonly Expression<Func<TItem, T>> _expression;

    public SortBy(Expression<Func<TItem, T>> expression)
    {
        _expression = expression;
    }

    public dynamic Expression => _expression;
}
