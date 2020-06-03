using System;

namespace Chef.Extensions.DbAccess.SqlServer.Extensions
{
    internal static class StringExtension
    {
        public static string Left(this string me, int length)
        {
            length = Math.Max(length, 0);

            return me.Length > length ? me.Substring(0, length) : me;
        }

        public static bool IsLikeOperator(this string me)
        {
            if (me.Equals("System.String.Contains")) return true;
            if (me.Equals("System.String.StartsWith")) return true;
            if (me.Equals("System.String.EndsWith")) return true;

            return false;
        }
    }
}