﻿using System;
using System.Collections.Generic;

namespace Chef.Extensions.LiteDB.Extensions
{
    internal static class DictionaryExtension
    {
        public static TValue SafeGetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> me, TKey key, Func<TValue> factory)
        {
            if (me.ContainsKey(key)) return me[key];

            lock (me)
            {
                if (me.ContainsKey(key)) return me[key];

                var value = factory();

                me.Add(key, value);

                return value;
            }
        }
    }
}