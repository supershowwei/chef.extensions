using System;
using System.Collections.Generic;

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

        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> me, TKey key, TValue defaultValue = default(TValue))
        {
            if (me == null) throw new ArgumentNullException(nameof(me));
            if (key == null) throw new ArgumentNullException(nameof(key));

            return me.TryGetValue(key, out var value) ? value : defaultValue;
        }
    }
}