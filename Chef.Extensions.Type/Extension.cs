using System.Linq;

namespace Chef.Extensions.Type
{
    public static class Extension
    {
        public static bool IsUserDefined(this System.Type me)
        {
            return !me.IsValueType && !me.IsPrimitive && (me.Namespace == null || !me.Namespace.StartsWith("System"));
        }

        public static string[] GetPropertyNames(this System.Type me)
        {
            return me.GetProperties().Select(x => x.Name).ToArray();
        }

        public static string[] GetPropertyNames(this System.Type me, string prefix)
        {
            return me.GetProperties().Select(x => string.Concat(prefix, x.Name)).ToArray();
        }
    }
}