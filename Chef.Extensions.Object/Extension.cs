using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Chef.Extensions.Object
{
    public static class Extension
    {
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
    }
}