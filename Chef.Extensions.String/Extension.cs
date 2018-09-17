using System;
using System.Collections.Generic;
using System.Linq;

namespace Chef.Extensions.String
{
    public static class Extension
    {
        public static bool IsNullOrEmpty(this string me)
        {
            return string.IsNullOrEmpty(me);
        }

        public static string Left(this string me, int length)
        {
            length = Math.Max(length, 0);

            return me.Length > length ? me.Substring(0, length) : me;
        }

        public static string Right(this string me, int length)
        {
            length = Math.Max(length, 0);

            return me.Length > length ? me.Substring(me.Length - length, length) : me;
        }

        public static bool EqualsIgnoreCase(
            this string me,
            string value,
            StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase)
        {
            return me != null && me.Equals(value, comparisonType);
        }

        public static bool StartsWithIgnoreCase(
            this string me,
            string value,
            StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase)
        {
            return me.StartsWith(value, comparisonType);
        }

        public static bool EndsWithIgnoreCase(
            this string me,
            string value,
            StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase)
        {
            return me.EndsWith(value, comparisonType);
        }

        public static bool ContainsIgnoreCase(this string me, string value)
        {
            return me != null && me.IndexOf(value, StringComparison.InvariantCultureIgnoreCase) >= 0;
        }

        public static List<T> Split<T>(this string me, char separator, Func<string, T> selector)
        {
            return me.Split(separator).Select(selector).ToList();
        }
    }
}