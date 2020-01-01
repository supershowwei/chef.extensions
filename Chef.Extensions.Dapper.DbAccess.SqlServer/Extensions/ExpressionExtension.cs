using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Chef.Extensions.Dapper.DbAccess.SqlServer.Extensions
{
    internal static class ExpressionExtension
    {
        public static string ToWhereStatement<T>(this Expression<Func<T, bool>> me, out IDictionary<string, object> parameters)
        {
            return ToWhereStatement(me, string.Empty, out parameters);
        }

        public static string ToWhereStatement<T>(this Expression<Func<T, bool>> me, string alias, out IDictionary<string, object> parameters)
        {
            parameters = new Dictionary<string, object>();

            if (me == null) return string.Empty;

            return string.Concat("\r\nWHERE ", me.ToSearchCondition(alias, parameters));
        }

        public static string ToOrderByStatement<T>(this IEnumerable<(Expression<Func<T, object>>, Sortord)> me)
        {
            return ToOrderByStatement(me, string.Empty);
        }

        public static string ToOrderByStatement<T>(this IEnumerable<(Expression<Func<T, object>>, Sortord)> me, string alias)
        {
            if (me == null) return string.Empty;
            if (!me.Any()) return string.Empty;

            var orderExpression = me.Select(
                o =>
                    {
                        var (expr, sortord) = o;

                        return sortord == Sortord.Descending ? expr.ToOrderDescending(alias) : expr.ToOrderAscending(alias);
                    });

            return string.Concat("\r\nORDER BY ", string.Join(", ", orderExpression));
        }
    }
}