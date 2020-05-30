using System;
using System.Collections.Generic;

namespace Chef.Extensions.DbAccess.SqlServer.Extensions
{
    internal static class IEnumerableExtension
    {
        public static int FindIndex<T>(this IEnumerable<T> me, Func<T, bool> predicate)
        {
            var index = 0;

            foreach (var item in me)
            {
                if (predicate(item)) return index;

                index++;
            }

            return -1;
        }
    }
}