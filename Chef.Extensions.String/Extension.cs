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

        public static bool IsNullOrWhiteSpace(this string me)
        {
            return string.IsNullOrWhiteSpace(me);
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

        public static bool EqualsIgnoreCase(this string me, string value)
        {
            return me.Equals(value, StringComparison.OrdinalIgnoreCase);
        }

        public static bool StartsWithIgnoreCase(this string me, string value)
        {
            return me.StartsWith(value, StringComparison.OrdinalIgnoreCase);
        }

        public static bool EndsWithIgnoreCase(this string me, string value)
        {
            return me.EndsWith(value, StringComparison.OrdinalIgnoreCase);
        }

        public static bool ContainsIgnoreCase(this string me, string value)
        {
            return me.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0;
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

        public static string[] SplitOmitEmptyEntries(this string me, params char[] separator)
        {
            return me.Split(separator, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string[] SplitOmitEmptyEntries(this string me, params string[] separator)
        {
            return me.Split(separator, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}