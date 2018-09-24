using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public static string Format(this string me, IDictionary dict)
        {
            var result = me;
            foreach (DictionaryEntry entry in dict)
            {
                result = me.Replace($"{{{entry.Key}}}", entry.Value.ToString());
            }

            return result;
        }

        public static string ToBase64(this string me)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(me));
        }

        public static string ToBase64(this string me, Encoding encoding)
        {
            return Convert.ToBase64String(encoding.GetBytes(me));
        }
    }
}