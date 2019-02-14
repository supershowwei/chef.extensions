# Chef.Extensions.Agility

A collection of useful extension methods without reference other packages.

## Chef.Extensions.Boolean

### public static T IIF&lt;T&gt;(this bool me, T trueValue, T falseValue)

Return `trueValue` if true, return `falseValue` if false.

### public static T IIF&lt;T&gt;(this bool me, Func&lt;T&gt; trueValue, Func&lt;T&gt; falseValue)

Execute and return `trueValue function` if true, execute and return `falseValue function` if false.

## Chef.Extensions.DateTime

### public static DateTime SetYear(this DateTime me, int year)

Change `Year` of DateTime.

### public static DateTime SetMonth(this DateTime me, int month)

Change `Month` of DateTime.

### public static DateTime SetDay(this DateTime me, int day)

Change `Day` of DateTime.

### public static DateTime SetHour(this DateTime me, int hour)

Change `Hour` of DateTime.

### public static DateTime SetMinute(this DateTime me, int minute)

Change `Minute` of DateTime.

### public static DateTime SetSecond(this DateTime me, int second)

Change `Second` of DateTime.

### public static DateTime StopYear(this DateTime me)

Keep `Year` value and set others as initail value.

Example:

    var datetime = new DateTime(2018, 12, 21, 12, 31, 44, 789).StopYear();
    
    // datetime is 2018/01/01 00:00:00.000

### public static DateTime StopMonth(this DateTime me)

Keep `Year`, `Month` value and set others as initail value.

Example:

    var datetime = new DateTime(2018, 12, 21, 12, 31, 44, 789).StopMonth();
    
    // datetime is 2018/12/01 00:00:00.000

### public static DateTime StopDay(this DateTime me)

Keep `Year`, `Month`, `Day` value and set others as initail value. It equals `DateTime.Date`.

Example:

    var datetime = new DateTime(2018, 12, 21, 12, 31, 44, 789).StopDay();
    
    // datetime is 2018/12/21 00:00:00.000

### public static DateTime StopHour(this DateTime me)

Keep `Year`, `Month`, `Day`, `Hour` value and set others as initail value.

Example:

    var datetime = new DateTime(2018, 12, 21, 12, 31, 44, 789).StopHour();
    
    // datetime is 2018/12/21 12:00:00.000

### public static DateTime StopMinute(this DateTime me)

Keep `Year`, `Month`, `Day`, `Hour` and `Minute` value and set others as initail value.

Example:

    var datetime = new DateTime(2018, 12, 21, 12, 31, 44, 789).StopMinute();
    
    // datetime is 2018/12/21 12:31:00.000

### public static DateTime StopSecond(this DateTime me)

Keep `Year`, `Month`, `Day`, `Hour`, `Minute`, `Second` value and set others as initail value.

Example:

    var datetime = new DateTime(2018, 12, 21, 12, 31, 44, 789).StopSecond();
    
    // datetime is 2018/12/21 12:31:44.000

### public static DateTime SpecifyDate(this DateTime me, int year, int month, int day)

Change `Year`, `Month` and `Day`.

Example:

    var datetime = new DateTime(2018, 12, 21, 12, 31, 44, 789).SpecifyDate(2019, 11, 12);
    
    // datetime is 2019/11/12 12:31:44.789

### public static DateTime SpecifyDate(this DateTime me, DateTime date)

Change `Year`, `Month`, `Day`.

Example:

    var datetime = new DateTime(2018, 12, 21, 12, 31, 44, 789).SpecifyDate(new DateTime(2019, 11, 12, 1, 1, 1, 111));
    
    // datetime is 2019/11/12 12:31:44.789

### public static DateTime SpecifyTime(this DateTime me, int hour, int minute, int second, int millisecond = 0)

Change `Hour`, `Minute`, `Second`, `Millisecond`（Default is 0）.

Example:

    var datetime = new DateTime(2018, 12, 21, 12, 31, 44, 789).SpecifyTime(1, 1, 1, 111));
    
    // datetime is 2018/12/21 01:01:01.111

### public static DateTime SpecifyTime(this DateTime me, DateTime time)

Change `Hour`, `Minute`, `Second`, `Millisecond`.

Example:

    var datetime = new DateTime(2018, 12, 21, 12, 31, 44, 789).SpecifyTime(new DateTime(2019, 11, 12, 1, 1, 1, 111));
    
    // datetime is 2018/12/21 01:01:01.111

### public static DateTime ToDateTime(this string me, string format)

It return `DateTime.ParseExact(me, format, CultureInfo.InvariantCulture)`.

### public static int DiffYears(this DateTime me, DateTime value)

Get date difference in `Years`.

### public static int DiffMonths(this DateTime me, DateTime value)

Get date difference in `Months`.

### public static int DiffDays(this DateTime me, DateTime value)

Get date difference in `Days`.

### public static int DiffHours(this DateTime me, DateTime value)

Get date difference in `Hours`.

### public static int DiffMinutes(this DateTime me, DateTime value)

Get date difference in `Minutes`.

### public static int DiffSeconds(this DateTime me, DateTime value)

Get date difference in `Seconds`.

## Chef.Extensions.Dictionary

### public static void AddRange&lt;TKey, TValue&gt;(this Dictionary&lt;TKey, TValue&gt; me, IDictionary&lt;TKey, TValue&gt; collection)

Add `IDictionary<TKey, TValue>` collection to Dictionary&lt;TKey, TValue&gt;.

### public static void AddRange&lt;TKey, TValue&gt;(this Dictionary&lt;TKey, TValue&gt; me, ICollection&lt;KeyValuePair&lt;TKey, TValue&gt;&gt; collection)

Add `ICollection<KeyValuePair<TKey, TValue>>` collection to Dictionary&lt;TKey, TValue&gt;.

### public static void AddRange&lt;TKey, TValue&gt;(this Dictionary&lt;TKey, TValue&gt; me, IEnumerable&lt;KeyValuePair&lt;TKey, TValue&gt;&gt; collection)

Add `IEnumerable<KeyValuePair<TKey, TValue>>` collection to Dictionary&lt;TKey, TValue&gt;.

## Chef.Extensions.Double

### public static double Round(this double me, MidpointRounding mode = MidpointRounding.AwayFromZero)

Round a double away from zero.

### public static double Round(this double me, int digits, MidpointRounding mode = MidpointRounding.AwayFromZero)

Round a double to `digits` places away from zero.

### public static int RoundUp(this double me)

Round up a double to int.

### public static double RoundUp(this double me, int digits)

Round up a double to `digits` places.

### public static int RoundDown(this double me)

Round down a double to int.

### public static double RoundDown(this double me, int digits)

Round down a double to `digits` places.

### public static int ToInt32(this double me)

Convert a double to int.

### public static long ToInt64(this double me)

Convert a double to long.

## Chef.Extensions.IEnumerable

### public static void ForEach&lt;T&gt;(this IEnumerable&lt;T&gt; me, Action&lt;T&gt; action)

Execute Action&lt;T&gt; iteratively.

### public static bool IsNullOrEmpty&lt;T&gt;(this IEnumerable&lt;T&gt; me)

Check if IEnumerable&lt;T&gt; is null or empty.

### public static bool Any&lt;T&gt;(this IEnumerable&lt;T&gt; me, Func&lt;T, bool&gt; predicate, out T result)

Check if any item predicated and return first predicated item.

### public static IEnumerable&lt;TResult&gt; SelectWhere&lt;T, TResult&gt;(this IEnumerable&lt;T&gt; me, Func&lt;T, bool&gt; predicate, Func&lt;T, TResult&gt; selector)

Select when predicated in one loop.

### public static IEnumerable&lt;TResult&gt; SelectWhere&lt;T, TResult&gt;(this IEnumerable&lt;T&gt; me, Func&lt;T, TResult, bool&gt; predicate)

Select out result when predicated in one loop.

Example:

    var numbers = new string[] { "1", "2", "three" }.SelectWhere((string s, out int result) => int.TryParse(s, out result));
    
    // numbers is [1, 2].

## Chef.Extensions.Object

### public static ExpandoObject ToExpando(this object me)

Convert a object to `ExpandoObject`.

## Chef.Extensions.String

### public static bool IsNullOrEmpty(this string me)

Check if string is null or empty.

### public static bool IsNullOrWhiteSpace(this string me)

Check if string is null, empty, or white space.

### public static string Left(this string me, int length)

Get characters of length from left.

### public static string Right(this string me, int length)

Get characters of length from right.

### public static bool EqualsIgnoreCase(this string me, string value)

`Equals` case insensitive.

### public static bool StartsWithIgnoreCase(this string me, string value)

`StartsWith` case insensitive.

### public static bool EndsWithIgnoreCase(this string me, string value)

`EndsWith` case insensitive.

### public static bool ContainsIgnoreCase(this string me, string value)

`Contains` case insensitive.

### public static T[] Split&lt;T&gt;(this string me, char separator, Func&lt;string, T&gt; selector)

Split string to T[].

### public static string Format(this string me, IDictionary dict)

Format string by replacing value matched `{Key}`.

Example:

    var oldValue = "abcd{Abc}";
    var replacements = new Dictionary<string, string>{ ["Abc"] = "eee" };
    
    var newValue = oldValue.Format(replacements);
    
    // newValue is "abcdeee".

### public static string ToBase64(this string me)

Encode a string to Base64.（Encoding is UTF8）

### public static string ToBase64(this string me, Encoding encoding)

Encode a string to Base64.

### public static string[] SplitOmitEmptyEntries(this string me, params char[] separator)

Split a string without empty entries using char separators.

### public static string[] SplitOmitEmptyEntries(this string me, params string[] separator)

Split a string without empty entries using string separators.

## Chef.Extensions.Type

### public static bool IsUserDefined(this Type me)

Check if Type is user defined.

### public static string[] GetPropertyNames(this Type me)

Return property names.

### public static string[] GetPropertyNames(this Type me, string prefix)

Return property names concatenated prefix.