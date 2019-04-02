using System;
using System.Collections.Concurrent;

namespace Chef.Extensions.Mvc.Helpers
{
    internal class ObjectLocker
    {
        private static readonly Lazy<ObjectLocker> Lazy = new Lazy<ObjectLocker>(() => new ObjectLocker());
        private readonly ConcurrentDictionary<string, object> locks = new ConcurrentDictionary<string, object>();

        private ObjectLocker()
        {
        }

        public static ObjectLocker Instance => Lazy.Value;

        public object GetLockObject(string key)
        {
            return this.locks.GetOrAdd(key, k => new object());
        }
    }
}