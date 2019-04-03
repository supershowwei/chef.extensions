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

        public static bool IsNotEmpty<T>(this IEnumerable<T> me)
        {
            return me != null && me.Any();
        }

        public static bool Any<T>(this IEnumerable<T> me, Func<T, bool> predicate, out T first)
        {
            foreach (var element in me)
            {
                if (predicate(element))
                {
                    first = element;

                    return true;
                }
            }

            first = default(T);

            return false;
        }

        public static bool Any<T>(this IEnumerable<T> me, Func<T, bool> predicate, out int index, out T first)
        {
            var i = 0;
            foreach (var element in me)
            {
                if (predicate(element))
                {
                    index = i;
                    first = element;

                    return true;
                }

                i++;
            }

            index = -1;
            first = default(T);

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

        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> me, int count)
        {
            Queue<T> queue;

            using (var enumerator = me.GetEnumerator())
            {
                if (!enumerator.MoveNext()) yield break;

                queue = new Queue<T>();
                queue.Enqueue(enumerator.Current);

                while (enumerator.MoveNext())
                {
                    if (queue.Count < count)
                    {
                        queue.Enqueue(enumerator.Current);
                    }
                    else
                    {
                        do
                        {
                            queue.Dequeue();
                            queue.Enqueue(enumerator.Current);
                        }
                        while (enumerator.MoveNext());

                        break;
                    }
                }
            }

            do
            {
                yield return queue.Dequeue();
            }
            while (queue.Count > 0);
        }

        public static IEnumerable<T> Merge<T>(
            this IEnumerable<T> me,
            IEnumerable<T> merged,
            System.Func<T, T, bool> compare,
            Action<T, T> merge)
        {
            var mergedList = new List<T>(merged);

            foreach (var element in me)
            {
                if (mergedList.Any(x => compare(x, element), out var index, out var existed))
                {
                    merge(element, existed);

                    mergedList.RemoveAt(index);
                }

                yield return element;
            }

            foreach (var left in mergedList)
            {
                yield return left;
            }
        }
    }
}