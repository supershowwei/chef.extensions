using System;
using System.Collections.Generic;
using System.Linq;

namespace Chef.Extensions.Dictionary
{
    public static class Extension
    {
        public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> me, IDictionary<TKey, TValue> collection)
        {
            foreach (var item in collection)
            {
                me.Add(item.Key, item.Value);
            }
        }

        public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> me, ICollection<KeyValuePair<TKey, TValue>> collection)
        {
            foreach (var pair in collection)
            {
                me.Add(pair.Key, pair.Value);
            }
        }

        public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> me, IEnumerable<KeyValuePair<TKey, TValue>> collection)
        {
            foreach (var pair in collection)
            {
                me.Add(pair.Key, pair.Value);
            }
        }

        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> me, TKey key, Func<TValue> factory)
        {
            if (!me.ContainsKey(key)) me.Add(key, factory());

            return me[key];
        }

        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> me, TKey key, Func<TKey, TValue> factory)
        {
            if (!me.ContainsKey(key)) me.Add(key, factory(key));

            return me[key];
        }

        public static TValue SafeGetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> me, TKey key, Func<TValue> factory)
        {
            if (!me.ContainsKey(key))
            {
                lock (me)
                {
                    if (!me.ContainsKey(key))
                    {
                        me.Add(key, factory());
                    }
                }
            }

            return me[key];
        }

        public static TValue SafeGetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> me, TKey key, Func<TKey, TValue> factory)
        {
            if (!me.ContainsKey(key))
            {
                lock (me)
                {
                    if (!me.ContainsKey(key))
                    {
                        me.Add(key, factory(key));
                    }
                }
            }

            return me[key];
        }

        public static TValue AddOrSet<TKey, TValue>(this IDictionary<TKey, TValue> me, TKey key, TValue value)
        {
            me[key] = value;

            return value;
        }

        public static TValue SafeAddOrSet<TKey, TValue>(this IDictionary<TKey, TValue> me, TKey key, TValue value)
        {
            lock (me)
            {
                me[key] = value;
            }

            return value;
        }

        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> me, TKey key, TValue defaultValue = default(TValue))
        {
            if (me == null) throw new ArgumentNullException(nameof(me));
            if (key == null) throw new ArgumentNullException(nameof(key));

            return me.TryGetValue(key, out var value) ? value : defaultValue;
        }

        public static void RollingRemove<TKey, TValue>(this IDictionary<TKey, TValue> me, int maxCount)
        {
            while (me.Count > maxCount)
            {
                me.Remove(me.Keys.Last());
            }
        }
    }
}