using System.Collections.Generic;
using System.Dynamic;

namespace Chef.Extensions.Object
{
    public static class Extension
    {
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