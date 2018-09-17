using System;
using System.Collections.Generic;

namespace Chef.Extensions.IEnumerable
{
    public static class Extension
    {
        public static void ForEach<T>(this IEnumerable<T> me, Action<T> action)
        {
            foreach (var element in me)
            {
                action(element);
            }
        }
    }
}