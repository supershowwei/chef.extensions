namespace Chef.Extensions.Type
{
    public static class Extension
    {
        public static bool IsUserDefinedType(this System.Type me)
        {
            return !me.IsValueType && !me.IsPrimitive && (me.Namespace == null || !me.Namespace.StartsWith("System"));
        }
    }
}