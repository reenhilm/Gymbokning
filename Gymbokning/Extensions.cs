using System.Linq.Expressions;

namespace Gymbokning
{
    public static class LinqExtensions
    {
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> whereClause)
        {
            if (condition)
            {
                return query.Where(whereClause);
            }
            return query;
        }
        public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> query, bool condition, Expression<Func<T, bool>> whereClause)
        {
            return query.AsQueryable().WhereIf<T>(condition, whereClause);
        }
    }
}
