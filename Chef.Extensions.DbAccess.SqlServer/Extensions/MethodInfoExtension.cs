using System.Reflection;

namespace Chef.Extensions.DbAccess.SqlServer.Extensions
{
    internal static class MethodInfoExtension
    {
        public static string GetFullName(this MethodInfo me)
        {
            var declaringType = me.DeclaringType;

            if (declaringType != null) return string.Concat(declaringType.FullName, ".", me.Name);

            return string.Empty;
        }
    }
}