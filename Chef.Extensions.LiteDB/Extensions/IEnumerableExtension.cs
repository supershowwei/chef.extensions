using System;
using System.Collections.Generic;

namespace Chef.Extensions.LiteDB.Extensions
{
    internal static class IEnumerableExtension
    {
        public static bool Any<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, out TSource result)
        {
            result = default(TSource);

            foreach (var item in source)
            {
                if (predicate(item))
                {
                    result = item;

                    return true;
                }
            }

            return false;
        }
    }
}