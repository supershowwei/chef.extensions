# Chef.Extensions.Agility

A collection of useful extension methods without reference other packages.

## Chef.Extensions.Boolean

### IIF&lt;T&gt;(T trueValue, T falseValue)

Return `trueValue` if true, return `falseValue` if false.

### IIF&lt;T&gt;(Func&lt;T&gt; trueValue, Func&lt;T&gt; falseValue)

Execute and return `trueValue function` if true, execute and return `falseValue function` if false.

## Chef.Extensions.DateTime

### SetYear(int year)

Change `Year` of DateTime.

### SetMonth(int month)

Change `Month` of DateTime.

### SetDay(int day)

Change `Day` of DateTime.

### SetHour(int hour)

Change `Hour` of DateTime.

### SetMinute(int minute)

Change `Minute` of DateTime.

### SetSecond(int second)

Change `Second` of DateTime.

### StopYear()

Keep `Year` value and set others as initail value.

Example:

    var datetime = new DateTime(2018, 12, 21, 12, 31, 44, 789).StopYear();
    
    // datetime is 2018/01/01 00:00:00.000

### StopMonth()

Keep `Year`, `Month` value and set others as initail value.

Example:

    var datetime = new DateTime(2018, 12, 21, 12, 31, 44, 789).StopMonth();
    
    // datetime is 2018/12/01 00:00:00.000

### StopDay()

Keep `Year`, `Month`, `Day` value and set others as initail value. It equals `DateTime.Date`.

Example:

    var datetime = new DateTime(2018, 12, 21, 12, 31, 44, 789).StopDay();
    
    // datetime is 2018/12/21 00:00:00.000

### StopHour()

Keep `Year`, `Month`, `Day`, `Hour` value and set others as initail value.

Example:

    var datetime = new DateTime(2018, 12, 21, 12, 31, 44, 789).StopHour();
    
    // datetime is 2018/12/21 12:00:00.000

### StopMinute()

Keep `Year`, `Month`, `Day`, `Hour` and `Minute` value and set others as initail value.

Example:

    var datetime = new DateTime(2018, 12, 21, 12, 31, 44, 789).StopMinute();
    
    // datetime is 2018/12/21 12:31:00.000

### StopSecond()

Keep `Year`, `Month`, `Day`, `Hour`, `Minute`, `Second` value and set others as initail value.

Example:

    var datetime = new DateTime(2018, 12, 21, 12, 31, 44, 789).StopSecond();
    
    // datetime is 2018/12/21 12:31:44.000

### SpecifyDate(int year, int month, int day)

Change `Year`, `Month` and `Day`.

Example:

    var datetime = new DateTime(2018, 12, 21, 12, 31, 44, 789).SpecifyDate(2019, 11, 12);
    
    // datetime is 2019/11/12 12:31:44.789

### SpecifyDate(DateTime date)

Change `Year`, `Month`, `Day`.

Example:

    var datetime = new DateTime(2018, 12, 21, 12, 31, 44, 789).SpecifyDate(new DateTime(2019, 11, 12, 1, 1, 1, 111));
    
    // datetime is 2019/11/12 12:31:44.789

### SpecifyTime(int hour, int minute, int second, int millisecond = 0)

Change `Hour`, `Minute`, `Second`, `Millisecond`（Default is 0）.

Example:

    var datetime = new DateTime(2018, 12, 21, 12, 31, 44, 789).SpecifyTime(1, 1, 1, 111));
    
    // datetime is 2018/12/21 01:01:01.111

### SpecifyTime(DateTime time)

Change `Hour`, `Minute`, `Second`, `Millisecond`.

Example:

    var datetime = new DateTime(2018, 12, 21, 12, 31, 44, 789).SpecifyTime(new DateTime(2019, 11, 12, 1, 1, 1, 111));
    
    // datetime is 2018/12/21 01:01:01.111

### ToDateTime(string format)

It return `DateTime.ParseExact(me, format, CultureInfo.InvariantCulture)`.

### DiffYears(DateTime value)

Get date difference in `Years`.

### DiffMonths(DateTime value)

Get date difference in `Months`.

### DiffDays(DateTime value)

Get date difference in `Days`.

### DiffHours(DateTime value)

Get date difference in `Hours`.

### DiffMinutes(DateTime value)

Get date difference in `Minutes`.

### DiffSeconds(DateTime value)

Get date difference in `Seconds`.

## Chef.Extensions.Dictionary

### AddRange&lt;TKey, TValue&gt;(this Dictionary&lt;TKey, TValue&gt; me, IDictionary&lt;TKey, TValue&gt; collection)

Add `IDictionary<TKey, TValue>` collection to Dictionary&lt;TKey, TValue&gt;.

### AddRange&lt;TKey, TValue&gt;(this Dictionary&lt;TKey, TValue&gt; me, ICollection&lt;KeyValuePair&lt;TKey, TValue&gt;&gt; collection)

Add `ICollection<KeyValuePair<TKey, TValue>>` collection to Dictionary&lt;TKey, TValue&gt;.

### AddRange&lt;TKey, TValue&gt;(this Dictionary&lt;TKey, TValue&gt; me, IEnumerable&lt;KeyValuePair&lt;TKey, TValue&gt;&gt; collection)

Add `IEnumerable<KeyValuePair<TKey, TValue>>` collection to Dictionary&lt;TKey, TValue&gt;.

## Chef.Extensions.Double

### Round(MidpointRounding mode = MidpointRounding.AwayFromZero)

Round a double away from zero.

### Round(int digits, MidpointRounding mode = MidpointRounding.AwayFromZero)

Round a double to `digits` places away from zero.

### RoundUp()

Round up a double to int.

### RoundUp(int digits)

Round up a double to `digits` places.

### RoundDown()

Round down a double to int.

### RoundDown(int digits)

Round down a double to `digits` places.

### ToInt32()

Convert a double to int.

### ToInt64()

Convert a double to long.

## Chef.Extensions.IEnumerable

### ForEach&lt;T&gt;(Action&lt;T&gt; action)

Execute Action&lt;T&gt; iteratively.

### IsNullOrEmpty&lt;T&gt;()

Check if IEnumerable&lt;T&gt; is null or empty.

### Any&lt;T&gt;(Func&lt;T, bool&gt; predicate, out T result)

Check if any item predicated and return first predicated item.

### IEnumerable&lt;TResult&gt; SelectWhere&lt;T, TResult&gt;(Func&lt;T, bool&gt; predicate, Func&lt;T, TResult&gt; selector)

Select when predicated in one loop.

### IEnumerable&lt;TResult&gt; SelectWhere&lt;T, TResult&gt;(Func&lt;T, TResult, bool&gt; predicate)

Select out result when predicated in one loop.

Example:

    var numbers = new string[] { "1", "2", "three" }.SelectWhere((string s, out int result) => int.TryParse(s, out result));
    
    // numbers is [1, 2].

## Chef.Extensions.Object

### ToExpando()

Convert a object to `ExpandoObject`.

## Chef.Extensions.String

### IsNullOrEmpty()

Check if string is null or empty.

### IsNullOrWhiteSpace()

Check if string is null, empty, or white space.

### Left(int length)

Get characters of length from left.

### Right(int length)

Get characters of length from right.

### EqualsIgnoreCase(string value)

`Equals` case insensitive.

### StartsWithIgnoreCase(string value)

`StartsWith` case insensitive.

### EndsWithIgnoreCase(string value)

`EndsWith` case insensitive.

### ContainsIgnoreCase(string value)

`Contains` case insensitive.

### Split&lt;T&gt;(char separator, Func&lt;string, T&gt; selector)

Split string to T[].

### Format(IDictionary dict)

Format string by replacing value matched `{Key}`.

Example:

    var oldValue = "abcd{Abc}";
    var replacements = new Dictionary<string, string>{ ["Abc"] = "eee" };
    
    var newValue = oldValue.Format(replacements);
    
    // newValue is "abcdeee".

### ToBase64()

Encode a string to Base64.（Encoding is UTF8）

### ToBase64(Encoding encoding)

Encode a string to Base64.

### SplitOmitEmptyEntries(params char[] separator)

Split a string without empty entries using char separators.

### SplitOmitEmptyEntries(params string[] separator)

Split a string without empty entries using string separators.

## Chef.Extensions.Type

### IsUserDefined()

Check if Type is user defined.

### GetPropertyNames()

Return property names.

### GetPropertyNames(string prefix)

Return property names concatenated prefix.

### IsMatch(string pattern, , RegexOptions options = RegexOptions.IgnoreCase)

Return true if regular expression matched, otherwise return false. Default is case insensitive.

### Match(this string me, string pattern, RegexOptions options = RegexOptions.IgnoreCase)

Return regular expression matched result. Default is case insensitive.

### Matches(this string me, string pattern, RegexOptions options = RegexOptions.IgnoreCase)

Return regular expression matched results. Default is case insensitive.