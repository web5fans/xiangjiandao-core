using System.Linq.Expressions;

namespace Xiangjiandao.Web.Extensions;

public static class IQueryableExtensions
{
    public static IOrderedQueryable<TSource> OrderByIf<TSource, TKey>(
        this IQueryable<TSource> source,
        bool condition,
        Expression<Func<TSource, TKey>> predicate,
        bool desc = false
    )
    {
        if (condition)
        {
            return desc ? source.OrderByDescending<TSource, TKey>(predicate) : source.OrderBy<TSource, TKey>(predicate);
        }

        return source.OrderBy(data => 0);
    }

    public static IOrderedQueryable<TSource> ThenByIf<TSource, TKey>(
        this IOrderedQueryable<TSource> source,
        bool condition,
        Expression<Func<TSource, TKey>> predicate,
        bool desc = false
    )
    {
        if (condition)
        {
            return desc ? source.ThenByDescending<TSource, TKey>(predicate) : source.ThenBy<TSource, TKey>(predicate);
        }

        return source;
    }
}