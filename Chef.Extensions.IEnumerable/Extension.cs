﻿using System;
using System.Collections.Generic;
using System.Linq;

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

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> me)
        {
            return me == null || !me.Any();
        }

        public static bool Any<T>(this IEnumerable<T> me, Func<T, bool> predicate, out T result)
        {
            result = default(T);

            foreach (var element in me)
            {
                if (predicate(element))
                {
                    result = element;

                    return true;
                }
            }

            return false;
        }
    }
}