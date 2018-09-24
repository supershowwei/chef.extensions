namespace Chef.Extensions.Dapper.Extensions
{
    internal static class TypeExtension
    {
        public static bool IsUserDefined(this System.Type me)
        {
            return !me.IsValueType && !me.IsPrimitive && (me.Namespace == null || !me.Namespace.StartsWith("System"));
        }
    }
}