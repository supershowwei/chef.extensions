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
    }
}