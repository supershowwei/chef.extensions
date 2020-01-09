using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Chef.Extensions.Object
{
    public static class Extension
    {
        private static readonly HashSet<System.Type> NumericTypes = new HashSet<System.Type>
                                                                    {
                                                                        typeof(byte),
                                                                        typeof(sbyte),
                                                                        typeof(short),
                                                                        typeof(ushort),
                                                                        typeof(int),
                                                                        typeof(uint),
                                                                        typeof(long),
                                                                        typeof(ulong),
                                                                        typeof(float),
                                                                        typeof(double),
                                                                        typeof(decimal),
                                                                    };

        public static bool IsNotNull(this object me)
        {
            return me != null;
        }

        public static T To<T>(this object me) where T : IConvertible
        {
            if (me == null) return default(T);

            return (T)Convert.ChangeType(me, typeof(T));
        }

        public static T? ToNullable<T>(this object me)
            where T : struct, IConvertible
        {
            if (me == null) return default(T?);

            return (T?)Convert.ChangeType(me, typeof(T));
        }

        public static ExpandoObject ToExpando(this object me)
        {
            IDictionary<string, object> expando = new ExpandoObject();

            foreach (var property in me.GetType().GetProperties())
            {
                expando.Add(property.Name, property.GetValue(me));
            }

            return (ExpandoObject)expando;
        }

        public static bool IsNumeric(this object me)
        {
            if (me == null) return false;

            return NumericTypes.Contains(me.GetType());
        }

        public static bool Exists<Ta, Tb>(this Ta me, IEnumerable<Tb> collection, Func<Ta, Tb, bool> predicate)
        {
            return collection.Any(item => predicate(me, item));
        }

        public static bool NotExists<Ta, Tb>(this Ta me, IEnumerable<Tb> collection, Func<Ta, Tb, bool> predicate)
        {
            return collection.All(item => !predicate(me, item));
        }
    }
}