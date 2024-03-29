﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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

        public static T[] Split<T>(this string me, char separator, Func<string, T> selector)
        {
            return me.Split(separator).Select(selector).ToArray();
        }

        public static T[] Split<T>(this string me, string separator, Func<string, T> selector)
        {
            return me.Split(new[] { separator }, StringSplitOptions.None).Select(selector).ToArray();
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

        public static string Concats(this string me, params string[] values)
        {
            var sb = new StringBuilder(me);

            foreach (var value in values)
            {
                sb.Append(value);
            }

            return sb.ToString();
        }

        public static string Base32Encode(this string me)
        {
            return Base32.Encode(me, Encoding.UTF8);
        }

        public static string Base32Encode(this string me, Encoding encoding)
        {
            return Base32.Encode(me, encoding);
        }

        public static string Base32Decode(this string me)
        {
            return Base32.Decode(me, Encoding.UTF8);
        }

        public static string Base32Decode(this string me, Encoding encoding)
        {
            return Base32.Decode(me, encoding);
        }

        public static string Base64Encode(this string me)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(me));
        }

        public static string Base64Encode(this string me, Encoding encoding)
        {
            return Convert.ToBase64String(encoding.GetBytes(me));
        }

        public static string Base64Decode(this string me)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(me));
        }

        public static string Base64Decode(this string me, Encoding encoding)
        {
            return encoding.GetString(Convert.FromBase64String(me));
        }

        public static string Base64UrlEncode(this string me)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(me)).Replace("+", "-").Replace("/", "_").Replace("=", string.Empty);
        }

        public static string Base64UrlEncode(this string me, Encoding encoding)
        {
            return Convert.ToBase64String(encoding.GetBytes(me)).Replace("+", "-").Replace("/", "_").Replace("=", string.Empty);
        }

        public static string Base64UrlDecode(this string me)
        {
            me = me.Replace("-", "+").Replace("_", "/");

            switch (me.Length % 4)
            {
                case 0:
                    break;

                case 2:
                    me = me.PadRight(me.Length + 2, '=');
                    break;

                case 3:
                    me = me.PadRight(me.Length + 1, '=');
                    break;

                default:
                    throw new ArgumentException("Incorrect string length.");
            }

            return Encoding.UTF8.GetString(Convert.FromBase64String(me));
        }

        public static string Base64UrlDecode(this string me, Encoding encoding)
        {
            me = me.Replace("-", "+").Replace("_", "/");

            switch (me.Length % 4)
            {
                case 0:
                    break;

                case 2:
                    me = me.PadRight(me.Length + 2, '=');
                    break;

                case 3:
                    me = me.PadRight(me.Length + 1, '=');
                    break;

                default:
                    throw new ArgumentException("Incorrect string length.");
            }

            return encoding.GetString(Convert.FromBase64String(me));
        }

        public static bool TryBase64UrlDecode(this string me, out string result)
        {
            result = default;

            try
            {
                me = me.Replace("-", "+").Replace("_", "/");

                switch (me.Length % 4)
                {
                    case 0:
                        break;

                    case 2:
                        me = me.PadRight(me.Length + 2, '=');
                        break;

                    case 3:
                        me = me.PadRight(me.Length + 1, '=');
                        break;

                    default:
                        throw new ArgumentException("Incorrect string length.");
                }

                result = Encoding.UTF8.GetString(Convert.FromBase64String(me));

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool TryBase64UrlDecode(this string me, Encoding encoding, out string result)
        {
            result = default;

            try
            {
                me = me.Replace("-", "+").Replace("_", "/");

                switch (me.Length % 4)
                {
                    case 0:
                        break;

                    case 2:
                        me = me.PadRight(me.Length + 2, '=');
                        break;

                    case 3:
                        me = me.PadRight(me.Length + 1, '=');
                        break;

                    default:
                        throw new ArgumentException("Incorrect string length.");
                }

                result = encoding.GetString(Convert.FromBase64String(me));

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string[] Split(this string me, params string[] separator)
        {
            return me.Split(separator, StringSplitOptions.None);
        }

        public static string[] SplitOmitEmptyEntries(this string me, params char[] separator)
        {
            return me.Split(separator, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string[] SplitOmitEmptyEntries(this string me, params string[] separator)
        {
            return me.Split(separator, StringSplitOptions.RemoveEmptyEntries);
        }

        public static bool IsMatch(this string me, string pattern)
        {
            return IsMatch(me, pattern, RegexOptions.IgnoreCase);
        }

        public static bool IsMatch(this string me, string pattern, RegexOptions options)
        {
            if (string.IsNullOrEmpty(me)) return false;

            return System.Text.RegularExpressions.Regex.IsMatch(me, pattern, options);
        }

        public static bool IsMatch(this string me, string pattern, out Match match)
        {
            return IsMatch(me, pattern, RegexOptions.IgnoreCase, out match);
        }

        public static bool IsMatch(this string me, string pattern, RegexOptions options, out Match match)
        {
            match = default(Match);

            if (string.IsNullOrEmpty(me)) return false;

            match = System.Text.RegularExpressions.Regex.Match(me, pattern, options);

            if (match.Success) return true;

            match = default(Match);

            return false;
        }

        public static Match Match(this string me, string pattern, RegexOptions options = RegexOptions.IgnoreCase)
        {
            if (string.IsNullOrEmpty(me)) return System.Text.RegularExpressions.Match.Empty;

            return System.Text.RegularExpressions.Regex.Match(me, pattern, options);
        }

        public static Match[] Matches(this string me, string pattern, RegexOptions options = RegexOptions.IgnoreCase)
        {
            if (string.IsNullOrEmpty(me)) return new Match[0];

            return System.Text.RegularExpressions.Regex.Matches(me, pattern, options).Cast<Match>().ToArray();
        }

        public static T ParseEnum<T>(this string me) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum) throw new ArgumentException("T must be an enumerated type.");

            return (T)System.Enum.Parse(typeof(T), me, true);
        }

        public static System.DateTime ToDateTime(this string me)
        {
            return System.DateTime.Parse(me);
        }

        public static System.DateTime ToDateTime(this string me, string format)
        {
            return System.DateTime.ParseExact(me, format, CultureInfo.InvariantCulture);
        }

        public static int ToInt32(this string me)
        {
            return int.Parse(me);
        }

        public static long ToInt64(this string me)
        {
            return long.Parse(me);
        }

        public static double ToDouble(this string me)
        {
            return double.Parse(me);
        }

        public static bool IsNumeric(this string me)
        {
            if (string.IsNullOrWhiteSpace(me)) return false;

            return decimal.TryParse(me, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out var result);
        }

        public static string Remove(this string me, params string[] values)
        {
            return values.Aggregate(me, (accu, next) => accu.Replace(next, string.Empty));
        }
    }
}