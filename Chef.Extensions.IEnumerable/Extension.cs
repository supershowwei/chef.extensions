using System;
using System.Collections.Generic;
using System.Linq;

namespace Chef.Extensions.IEnumerable
{
    public delegate TReturn Func<in T, TResult, out TReturn>(T arg, out TResult result);

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

        public static IEnumerable<TResult> SelectWhere<T, TResult>(
            this IEnumerable<T> me,
            Func<T, bool> predicate,
            Func<T, TResult> selector)
        {
            foreach (var item in me)
            {
                if (predicate(item)) yield return selector(item);
            }
        }

        public static IEnumerable<TResult> SelectWhere<T, TResult>(
            this IEnumerable<T> me,
            Func<T, TResult, bool> predicate)
        {
            foreach (var item in me)
            {
                if (predicate(item, out var result)) yield return result;
            }
        }
    }
}