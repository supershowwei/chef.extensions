using System.Collections.Generic;

namespace Chef.Extensions.Dapper.Extensions
{
    internal static class DictionaryExtension
    {
        public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> me, Dictionary<TKey, TValue> collection)
        {
            if (collection == null || collection.Count == 0) return;

            foreach (var item in collection)
            {
                me.Add(item.Key, item.Value);
            }
        }
    }
}