# Chef.Extensions.Agility

A collection of useful extension methods without reference other packages.

## Chef.Extensions.Boolean

### IIF&lt;T&gt;(this bool me, T trueValue, T falseValue)

Return `trueValue` if true, return `falseValue` if false.

### IIF&lt;T&gt;(this bool me, Func&lt;T&gt; trueValue, Func&lt;T&gt; falseValue)

Execute and return `trueValue function` if true, execute and return `falseValue function` if false.

## Chef.Extensions.DateTime

### SetYear(this DateTime me, int year)

Change `Year` of DateTime.

### SetMonth(this DateTime me, int month)

Change `Month` of DateTime.

### SetDay(this DateTime me, int day)

Change `Day` of DateTime.

### SetHour(this DateTime me, int hour)

Change `Hour` of DateTime.

### SetMinute(this DateTime me, int minute)

Change `Minute` of DateTime.

### SetSecond(this DateTime me, int second)

Change `Second` of DateTime.

### StopYear(this DateTime me)

Keep `Year` value and set others as initail value.

Example:

    var datetime = new DateTime(2018, 12, 21, 12, 31, 44, 789).StopYear();
    
    // datetime is 2018/01/01 00:00:00.000

### StopMonth(this DateTime me)

Keep `Year`, `Month` value and set others as initail value.

Example:

    var datetime = new DateTime(2018, 12, 21, 12, 31, 44, 789).StopMonth();
    
    // datetime is 2018/12/01 00:00:00.000

### StopDay(this DateTime me)

Keep `Year`, `Month`, `Day` value and set others as initail value. It equals `DateTime.Date`.

Example:

    var datetime = new DateTime(2018, 12, 21, 12, 31, 44, 789).StopDay();
    
    // datetime is 2018/12/21 00:00:00.000

### StopHour(this DateTime me)

Keep `Year`, `Month`, `Day`, `Hour` value and set others as initail value.

Example:

    var datetime = new DateTime(2018, 12, 21, 12, 31, 44, 789).StopHour();
    
    // datetime is 2018/12/21 12:00:00.000

### StopMinute(this DateTime me)

Keep `Year`, `Month`, `Day`, `Hour` and `Minute` value and set others as initail value.

Example:

    var datetime = new DateTime(2018, 12, 21, 12, 31, 44, 789).StopMinute();
    
    // datetime is 2018/12/21 12:31:00.000

### StopSecond(this DateTime me)

Keep `Year`, `Month`, `Day`, `Hour`, `Minute`, `Second` value and set others as initail value.

Example:

    var datetime = new DateTime(2018, 12, 21, 12, 31, 44, 789).StopSecond();
    
    // datetime is 2018/12/21 12:31:44.000

### SpecifyDate(this DateTime me, int year, int month, int day)

Change `Year`, `Month` and `Day`.

Example:

    var datetime = new DateTime(2018, 12, 21, 12, 31, 44, 789).SpecifyDate(2019, 11, 12);
    
    // datetime is 2019/11/12 12:31:44.789

### SpecifyDate(this DateTime me, DateTime date)

Change `Year`, `Month`, `Day`.

Example:

    var datetime = new DateTime(2018, 12, 21, 12, 31, 44, 789).SpecifyDate(new DateTime(2019, 11, 12, 1, 1, 1, 111));
    
    // datetime is 2019/11/12 12:31:44.789

### SpecifyTime(this DateTime me, int hour, int minute, int second, int millisecond = 0)

Change `Hour`, `Minute`, `Second`, `Millisecond`（Default is 0）.

Example:

    var datetime = new DateTime(2018, 12, 21, 12, 31, 44, 789).SpecifyTime(1, 1, 1, 111));
    
    // datetime is 2018/12/21 01:01:01.111

### SpecifyTime(this DateTime me, DateTime time)

Change `Hour`, `Minute`, `Second`, `Millisecond`.

Example:

    var datetime = new DateTime(2018, 12, 21, 12, 31, 44, 789).SpecifyTime(new DateTime(2019, 11, 12, 1, 1, 1, 111));
    
    // datetime is 2018/12/21 01:01:01.111

### ToDateTime(this string me, string format)

It return `DateTime.ParseExact(me, format, CultureInfo.InvariantCulture)`.

### DiffYears(this DateTime me, DateTime value)

Get date difference in `Years`.

### DiffMonths(this DateTime me, DateTime value)

Get date difference in `Months`.

### DiffDays(this DateTime me, DateTime value)

Get date difference in `Days`.

### DiffHours(this DateTime me, DateTime value)

Get date difference in `Hours`.

### DiffMinutes(this DateTime me, DateTime value)

Get date difference in `Minutes`.

### DiffSeconds(this DateTime me, DateTime value)

Get date difference in `Seconds`.

## Chef.Extensions.Dictionary

### AddRange&lt;TKey, TValue&gt;(this Dictionary&lt;TKey, TValue&gt; me, IDictionary&lt;TKey, TValue&gt; collection)

Add `IDictionary<TKey, TValue>` collection to Dictionary&lt;TKey, TValue&gt;.

### AddRange&lt;TKey, TValue&gt;(this Dictionary&lt;TKey, TValue&gt; me, ICollection&lt;KeyValuePair&lt;TKey, TValue&gt;&gt; collection)

Add `ICollection<KeyValuePair<TKey, TValue>>` collection to Dictionary&lt;TKey, TValue&gt;.

### AddRange&lt;TKey, TValue&gt;(this Dictionary&lt;TKey, TValue&gt; me, IEnumerable&lt;KeyValuePair&lt;TKey, TValue&gt;&gt; collection)

Add `IEnumerable<KeyValuePair<TKey, TValue>>` collection to Dictionary&lt;TKey, TValue&gt;.

## Chef.Extensions.Double

### Round(this double me, MidpointRounding mode = MidpointRounding.AwayFromZero)

Round a double away from zero.

### Round(this double me, int digits, MidpointRounding mode = MidpointRounding.AwayFromZero)

Round a double to `digits` places away from zero.

### RoundUp(this double me)

Round up a double to int.

### RoundUp(this double me, int digits)

Round up a double to `digits` places.

### RoundDown(this double me)

Round down a double to int.

### RoundDown(this double me, int digits)

Round down a double to `digits` places.

### ToInt32(this double me)

Convert a double to int.

### ToInt64(this double me)

Convert a double to long.

## Chef.Extensions.IEnumerable

### ForEach&lt;T&gt;(this IEnumerable&lt;T&gt; me, Action&lt;T&gt; action)

Execute Action&lt;T&gt; iteratively.

### IsNullOrEmpty&lt;T&gt;(this IEnumerable&lt;T&gt; me)

Check if IEnumerable&lt;T&gt; is null or empty.

### Any&lt;T&gt;(this IEnumerable&lt;T&gt; me, Func&lt;T, bool&gt; predicate, out T result)

Check if any item predicated and return first predicated item.

### IEnumerable&lt;TResult&gt; SelectWhere&lt;T, TResult&gt;(this IEnumerable&lt;T&gt; me, Func&lt;T, bool&gt; predicate, Func&lt;T, TResult&gt; selector)

Select when predicated in one loop.

### IEnumerable&lt;TResult&gt; SelectWhere&lt;T, TResult&gt;(this IEnumerable&lt;T&gt; me, Func&lt;T, TResult, bool&gt; predicate)

Select out result when predicated in one loop.

Example:

    var numbers = new string[] { "1", "2", "three" }.SelectWhere((string s, out int result) => int.TryParse(s, out result));
    
    // numbers is [1, 2].

## Chef.Extensions.Object

### ToExpando(this object me)

Convert a object to `ExpandoObject`.

## Chef.Extensions.String

### IsNullOrEmpty(this string me)

Check if string is null or empty.

### IsNullOrWhiteSpace(this string me)

Check if string is null, empty, or white space.

### Left(this string me, int length)

Get characters of length from left.

### Right(this string me, int length)

Get characters of length from right.

### EqualsIgnoreCase(this string me, string value)

`Equals` case insensitive.

### StartsWithIgnoreCase(this string me, string value)

`StartsWith` case insensitive.

### EndsWithIgnoreCase(this string me, string value)

`EndsWith` case insensitive.

### ContainsIgnoreCase(this string me, string value)

`Contains` case insensitive.

### Split&lt;T&gt;(this string me, char separator, Func&lt;string, T&gt; selector)

Split string to T[].

### Format(this string me, IDictionary dict)

Format string by replacing value matched `{Key}`.

Example:

    var oldValue = "abcd{Abc}";
    var replacements = new Dictionary<string, string>{ ["Abc"] = "eee" };
    
    var newValue = oldValue.Format(replacements);
    
    // newValue is "abcdeee".

### ToBase64(this string me)

Encode a string to Base64.（Encoding is UTF8）

### ToBase64(this string me, Encoding encoding)

Encode a string to Base64.

### SplitOmitEmptyEntries(this string me, params char[] separator)

Split a string without empty entries using char separators.

### SplitOmitEmptyEntries(this string me, params string[] separator)

Split a string without empty entries using string separators.

## Chef.Extensions.Type

### IsUserDefined(this Type me)

Check if Type is user defined.

### GetPropertyNames(this Type me)

Return property names.

### GetPropertyNames(this Type me, string prefix)

Return property names concatenated prefix.