# Chef.Extensions.Agility

A collection of useful extension methods without reference other packages.

## Chef.Extensions.Assembly

### GetCurrentDirectory()

Return directory of assembly executing location.

Example:

    var result = Assembly.GetExecutingAssembly().GetCurrentDirectory();
    
    // result is current directory of current assembly.

## Chef.Extensions.Boolean

### IIF&lt;T&gt;(T trueValue, T falseValue)

Return `trueValue` if true, return `falseValue` if false.

Example:

    var price = -1;
    var result = (price > 0).IIF("red", "green");
    
    // result is "green".

### IIF&lt;T&gt;(Func&lt;T&gt; trueValue, Func&lt;T&gt; falseValue)

Execute and return `trueValue function` if true, execute and return `falseValue function` if false.

Example:

    var price = -1;
    var result = (price > 0).IIF(() => "red", () => "green");
    
    // result is "green".

## Chef.Extensions.Byte

### GetRange(int startIndex, int length)

Copy range of byte arrary.

Example:

    var data = new byte[] { 0x01, 0x02, 0x03 0x04, 0x05 };
    
    var bytes = data.GetRange(1, 2);
    
    // bytes is [0x02, 0x03].

### GetRange(long startIndex, long length)

Copy range of byte arrary.

Example:

    var data = new byte[] { 0x01, 0x02, 0x03 0x04, 0x05 };
    
    var bytes = data.GetRange(2, 2);
    
    // bytes is [0x03, 0x04].

### Set(int index, byte value)

Set value by index.

Example:

    var data = new byte[] { 0x01, 0x02, 0x03 0x04, 0x05 };
    
    data.Set(1, 0x06);
    
    // bytes is [0x01, 0x06, 0x03 0x04, 0x05].

### Set(long index, byte value)

Set value by index.

Example:

    var data = new byte[] { 0x01, 0x02, 0x03 0x04, 0x05 };
    
    data.Set(1, 0x06);
    
    // bytes is [0x01, 0x06, 0x03 0x04, 0x05].

### SetLast(byte value)

Set last value.

Example:

    var data = new byte[] { 0x01, 0x02, 0x03 0x04, 0x05 };
    
    data.SetLast(0x06);
    
    // bytes is [0x01, 0x02, 0x03 0x04, 0x06].

## Chef.Extensions.DateTime

### SetYear(int year)

Change `Year` of DateTime.

Example:

    var result = new DateTime(2018, 1, 12, 12, 34, 56).SetYear(2017);
    
    // result is 2017/1/12 12:34:56

### SetMonth(int month)

Change `Month` of DateTime.

Example:

    var result = new DateTime(2018, 1, 12, 12, 34, 56).SetMonth(2);
    
    // result is 2018/2/12 12:34:56

### SetDay(int day)

Change `Day` of DateTime.

Example:

    var result = new DateTime(2018, 1, 12, 12, 34, 56).SetDay(13);
    
    // result is 2018/1/13 12:34:56

### SetHour(int hour)

Change `Hour` of DateTime.

Example:

    var result = new DateTime(2018, 1, 12, 12, 34, 56).SetHour(14);
    
    // result is 2018/1/12 14:34:56

### SetMinute(int minute)

Change `Minute` of DateTime.

Example:

    var result = new DateTime(2018, 1, 12, 12, 34, 56).SetMinute(45);
    
    // result is 2018/1/12 12:45:56

### SetSecond(int second)

Change `Second` of DateTime.

Example:

    var result = new DateTime(2018, 1, 12, 12, 34, 56).SetSecond(23);
    
    // result is 2018/1/12 12:34:23

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

### SetStopYear(int year)

Change `Year` of DateTime, keep `Year` value and set others as initail value.

Example:

    var result = new DateTime(2018, 1, 12, 12, 34, 56).SetStopYear(2017);
    
    // result is 2017/1/1 00:00:00

### SetStopMonth(int month)

Change `Month` of DateTime, keep `Year`, `Month` value and set others as initail value.

Example:

    var result = new DateTime(2018, 1, 12, 12, 34, 56).SetStopMonth(2);
    
    // result is 2018/2/1 00:00:00

### SetStopDay(int day)

Change `Day` of DateTime, keep `Year`, `Month`, `Day` value and set others as initail value. It equals `DateTime.Date`.

Example:

    var result = new DateTime(2018, 1, 12, 12, 34, 56).SetStopDay(13);
    
    // result is 2018/1/13 00:00:00

### SetStopHour(int hour)

Change `Hour` of DateTime, keep `Year`, `Month`, `Day`, `Hour` value and set others as initail value.

Example:

    var result = new DateTime(2018, 1, 12, 12, 34, 56).SetStopHour(14);
    
    // result is 2018/1/12 14:00:00

### SetStopMinute(int minute)

Change `Minute` of DateTime, keep `Year`, `Month`, `Day`, `Hour` and `Minute` value and set others as initail value.

Example:

    var result = new DateTime(2018, 1, 12, 12, 34, 56).SetStopMinute(45);
    
    // result is 2018/1/12 12:45:00

### SetStopSecond(int second)

Change `Second` of DateTime, keep `Year`, `Month`, `Day`, `Hour`, `Minute`, `Second` value and set others as initail value.

Example:

    var result = new DateTime(2018, 1, 12, 12, 34, 56).SetStopSecond(23);
    
    // result is 2018/1/12 12:34:23.000

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

### DiffYears(DateTime value)

Get date difference in `Years`.

Example:

    var result = new DateTime(2018, 1, 2).DiffYears(new DateTime(2019, 2, 1));
    
    // result is 1.

### DiffMonths(DateTime value)

Get date difference in `Months`.

Example:

    var result = new DateTime(2018, 1, 2).DiffMonths(new DateTime(2019, 2, 1));
    
    // result is 13.

### DiffDays(DateTime value)

Get date difference in `Days`.

Example:

    var result = new DateTime(2018, 1, 2).DiffDays(new DateTime(2019, 2, 1));
    
    // result is 395.

### DiffHours(DateTime value)

Get date difference in `Hours`.

Example:

    var result = new DateTime(2018, 1, 2, 12, 34, 56).DiffHours(new DateTime(2019, 2, 1, 1, 23, 45));
    
    // result is 9469

### DiffMinutes(DateTime value)

Get date difference in `Minutes`.

Example:

    var result = new DateTime(2018, 1, 2, 12, 34, 56).DiffMinutes(new DateTime(2019, 2, 1, 1, 23, 45));
    
    // result is 568129.

### DiffSeconds(DateTime value)

Get date difference in `Seconds`.

Example:

    var result = new DateTime(2018, 1, 2, 12, 34, 56).DiffSeconds(new DateTime(2019, 2, 1, 1, 23, 45));
    
    // result is 34087729.

### ToJavaScriptTime()

Convert to milliseconds of JavaScript time.

Example:

    var time = new DateTime(2019, 5, 7, 10, 23, 55);
    
    var jsTime = time.ToJavaScriptTime();
    
    // jsTime is 1557224635000.

## Chef.Extensions.Dictionary

### AddRange&lt;TKey, TValue&gt;(IDictionary&lt;TKey, TValue&gt; collection)

Add `IDictionary<TKey, TValue>` collection to Dictionary&lt;TKey, TValue&gt;.

Example:

    var dict = new Dictionary<string, int> { { "1", 1 }, { "2", 2 } };
    var anothers = (IDictionary<string, int>)new Dictionary<string, int> { { "3", 3 }, { "4", 4 } };
    
    dict.AddRange(anothers);
    
    // dict is {"1":1,"2":2,"3":3,"4":4}.

### AddRange&lt;TKey, TValue&gt;(ICollection&lt;KeyValuePair&lt;TKey, TValue&gt;&gt; collection)

Add `ICollection<KeyValuePair<TKey, TValue>>` collection to Dictionary&lt;TKey, TValue&gt;.

Example:

    var dict = new Dictionary<string, int> { { "1", 1 }, { "2", 2 } };
    var anothers = (ICollection<KeyValuePair<string, int>>)new Dictionary<string, int> { { "3", 3 }, { "4", 4 } };
    
    dict.AddRange(anothers);
    
    // dict is {"1":1,"2":2,"3":3,"4":4}.

### AddRange&lt;TKey, TValue&gt;(IEnumerable&lt;KeyValuePair&lt;TKey, TValue&gt;&gt; collection)

Add `IEnumerable<KeyValuePair<TKey, TValue>>` collection to Dictionary&lt;TKey, TValue&gt;.

Example:

    var dict = new Dictionary<string, int> { { "1", 1 }, { "2", 2 } };
    var anothers = (IEnumerable<KeyValuePair<string, int>>)new Dictionary<string, int> { { "3", 3 }, { "4", 4 } };
    
    dict.AddRange(anothers);
    
    // dict is {"1":1,"2":2,"3":3,"4":4}.

### GetOrAdd&lt;TKey, TValue&gt;(TKey key, Func&lt;TValue&gt; factory)

Get value by key, but create object if not exists.

Example:

    var dict = new Dictionary<string, int>();
    
    var value = dict.GetOrAdd("1", () => 1);
    
    // value is 1.

### SafeGetOrAdd&lt;TKey, TValue&gt;(TKey key, Func&lt;TValue&gt; factory)

Get value by key, but create object if not exists. [Thread-Safe]

Example:

    var dict = new Dictionary<string, int>();
    
    var value = dict.SafeGetOrAdd("1", () => 1);
    
    // value is 1.

## Chef.Extensions.Double

### Round(MidpointRounding mode = MidpointRounding.AwayFromZero)

Round a int away from zero.

Example:

    var num1 = 1.4.Round();
    var num2 = 1.5.Round();
    var num3 = 1.6.Round();
    
    // num1 = 1, num2 = 2, num3 = 2

### Round(int digits, MidpointRounding mode = MidpointRounding.AwayFromZero)

Round a double to `digits` places away from zero.

Example:

    var num1 = 1.44.Round(1);
    var num2 = 1.55.Round(1);
    var num3 = 1.66.Round(1);
    
    // num1 = 1.4, num2 = 1.6, num3 = 1.7

### RoundUp()

Round up a double to int.

Example:

    var result = 1.44.RoundUp();

    // result is 2.

### RoundUp(int digits)

Round up a double to `digits` places.

Example:

    var num1 = 1.44.RoundUp(1);
    var num2 = 1.55.RoundUp(1);
    var num3 = 1.66.RoundUp(1);
    
    // num1 = 1.5, num2 = 1.6, num3 = 1.7

### RoundDown()

Round down a double to int.

Example:

    var result = 1.44.RoundDown();

    // result is 1.

### RoundDown(int digits)

Round down a double to `digits` places.

Example:

    var num1 = 1.44.RoundDown(1);
    var num2 = 1.55.RoundDown(1);
    var num3 = 1.66.RoundDown(1);
    
    // num1 = 1.4, num2 = 1.5, num3 = 1.6

### ToInt32()

Convert a double to int.

Example:

    var num1 = 1.4.ToInt32();
    var num2 = 1.5.ToInt32();
    var num3 = 1.6.ToInt32();
    
    // num1 = 1, num2 = 2, num3 = 2

### ToInt64()

Convert a double to long.

Example:

    var num1 = 1.4.ToInt64();
    var num2 = 1.5.ToInt64();
    var num3 = 1.6.ToInt64();
    
    // num1 = 1, num2 = 2, num3 = 2

## Chef.Extensions.Enum

### GetDescription()

Return description if Enum field has DescriptionAttribute, otherwise return enum name.

Example:

    public enum SomeKind
    {
        [Description("A new one.")]
        New,
        Old
    }

    var result1 = SomeKind.New.GetDescription();
    var result2 = SomeKind.Old.GetDescription();
    
    // result1 is "A new one.".
    // result2 is "Old".

## Chef.Extensions.IEnumerable

### ForEach&lt;T&gt;(Action&lt;T&gt; action)

Execute Action&lt;T&gt; iteratively.

Example:

    public class Person
    {
        public string Name { get; set; }
    
        public int Age { get; set; }
    }

    var result = new[] { new Person { Name = "abc", Age = 1 }, new Person { Name = "def", Age = 2 } };
    
    result.ForEach(x => x.Age += 1);
    
    // result is [{"Name":"abc","Age":2},{"Name":"def","Age":3}]

### IsNullOrEmpty&lt;T&gt;()

Check if IEnumerable&lt;T&gt; is null or empty.

Example:

    var result = new int[] { }.IsNullOrEmpty();
    
    // result is true.

### IsNotEmpty&lt;T&gt;()

Check if IEnumerable&lt;T&gt; is not null and not empty.

Example:

    var result = new int[] { }.IsNotEmpty();
    
    // result is false.

### Any&lt;T&gt;(Func&lt;T, bool&gt; predicate, out T first)

Check if any item predicated and output first predicated item.

Example:

    public class Person
    {
        public string Name { get; set; }
    
        public int Age { get; set; }
    }

    var persons = new[]
                  {
                      new Person { Name = "abc", Age = 1 },
                      new Person { Name = "def", Age = 2 },
                      new Person { Name = "ghi", Age = 2 }
                  };

    persons.Any(x => x.Age == 2, out var result);
    
    // result is {"Name":"def","Age":2}

### Any&lt;T&gt;(Func&lt;T, bool&gt; predicate, out int index, out T first)

Check if any item predicated and output index and first predicated item.

Example:

    public class Person
    {
        public string Name { get; set; }
    
        public int Age { get; set; }
    }

    var persons = new[]
                  {
                      new Person { Name = "abc", Age = 1 },
                      new Person { Name = "def", Age = 2 },
                      new Person { Name = "ghi", Age = 2 }
                  };

    persons.Any(x => x.Age == 2, out var index, out var first);
    
    // index is 1.
    // first is {"Name":"def","Age":2}.

### SelectWhere&lt;T, TResult&gt;(Func&lt;T, bool&gt; predicate, Func&lt;T, TResult&gt; selector)

Select when predicated in one loop.

Example:

    public class Person
    {
        public string Name { get; set; }
    
        public int Age { get; set; }
    }

    var persons = new[]
                  {
                      new Person { Name = "abc", Age = 1 },
                      new Person { Name = "def", Age = 2 },
                      new Person { Name = "ghi", Age = 2 }
                  };

    var result = persons.SelectWhere(x => x.Age == 2, x => x);
    
    // result is [{"Name":"def","Age":2},{"Name":"ghi","Age":2}]

### SelectWhere&lt;T, TResult&gt;(Func&lt;T, TResult, bool&gt; predicate)

Select out result when predicated in one loop.

Example:

    var numbers = new string[] { "1", "2", "three" }.SelectWhere((string s, out int result) => int.TryParse(s, out result));
    
    // numbers is [1, 2].

### TakeLast&lt;T&gt;(int count)

Returns a specified number of contiguous elements from the end of a original sequence.

Example:

    public class Person
    {
        public string Name { get; set; }
    
        public int Age { get; set; }
    }

    var persons = new[]
                  {
                      new Person { Name = "abc", Age = 1 },
                      new Person { Name = "def", Age = 2 },
                      new Person { Name = "ghi", Age = 2 }
                  };

    var result = persons.TakeLast(1);
    
    // result is [{"Name":"ghi","Age":2}]

### Merge&lt;T&gt;(IEnumerable&lt;T&gt; merged, Func&lt;T, T, bool&gt; compare, Action&lt;T, T&gt; merge)

Merge two collection into one.

Example:

    public class Person
    {
        public string Name { get; set; }
    
        public int Age { get; set; }
    }

    var primaryPersons = new[]
                         {
                             new Person { Name = "abc" },
                             new Person { Name = "def" }
                         };

    var mergedPersons = new[]
                        {
                            new Person { Name = "abc", Age = 1 },
                            new Person { Name = "def", Age = 2 },
                            new Person { Name = "ghi", Age = 3 }
                        };

    var result = primaryPersons.Merge(
        mergedPersons,
        (primary, merged) => primary.Name.Equals(merged.Name),
        (primary, merged) => { primary.Age = merged.Age; });
    
    // result is [{"Name": "abc", "Age": 1}, {"Name": "def", "Age": 2}, {"Name": "ghi", "Age": 3}]

### FindIndex&lt;T&gt;(Func&lt;T, bool&gt; predicate)

Find index by predicate.

Example:

    IEnumerable<int> enumerable = new[] { 11, 22, 33, 44, 55 };

    var result = enumerable.FindIndex(x => x.Equals(55));

    // result is 4.

## Chef.Extensions.Long

### ToDateTime()

Parse JavaScript time to DateTime.

Example:

    var milliseconds = 1557224635000;
    
    var time = milliseconds.ToDateTime();
    
    // time is 2019/05/07 10:23:55

## Chef.Extensions.Object

### IsNotNull()

Return true if object is null, otherwise false.

Example:

    var result = new object().IsNotNull();
    
    // result is true.

### ToExpando()

Convert a object to `ExpandoObject`.

Example:

    var result = new { Name = "abc", Age = 2 }.ToExpando();
    
    // result is {"Name":"abc","Age":2}

## Chef.Extensions.String

### IsNullOrEmpty()

Check if string is null or empty.

Example:

    var result = "".IsNullOrEmpty();
    
    // result is true.

### IsNullOrWhiteSpace()

Check if string is null, empty, or white space.

Example:

    var result = " \t\r\n".IsNullOrWhiteSpace();
    
    // result is true.

### Left(int length)

Get characters of length from left.

Example:

    var result = "abcdefg".Left(3);
    
    // result is "abc".

### Right(int length)

Get characters of length from right.

Example:

    var result = "abcdefg".Right(3);
    
    // result is "efg".

### EqualsIgnoreCase(string value)

`Equals` case insensitive.

Example:

    var result = "abcdefg".EqualsIgnoreCase("AbcDefG");
    
    // result is true.

### StartsWithIgnoreCase(string value)

`StartsWith` case insensitive.

Example:

    var result = "abcdefg".StartsWithIgnoreCase("AbC");
    
    // result is true.

### EndsWithIgnoreCase(string value)

`EndsWith` case insensitive.

Example:

    var result = "abcdefg".EndsWithIgnoreCase("FG");
    
    // result is true.

### ContainsIgnoreCase(string value)

`Contains` case insensitive.

Example:

    var result = "abcdefg".ContainsIgnoreCase("cDe");
    
    // result is true.

### Split&lt;T&gt;(char separator, Func&lt;string, T&gt; selector)

Split string to T[] by char separator.

Example:

    var result = "1,2,3,4".Split(',', x => int.Parse(x));
    
    // result is [1,2,3,4].

### Split&lt;T&gt;(string separator, Func&lt;string, T&gt; selector)

Split string to T[] by string separator.

Example:

    var result = "1,,2,,3,,4".Split(",,", x => int.Parse(x));
    
    // result is [1,2,3,4].

### Format(IDictionary dict)

Format string by replacing value matched `{Key}`.

Example:

    var oldValue = "abcd{Abc}";
    var replacements = new Dictionary<string, string>{ ["Abc"] = "eee" };
    
    var newValue = oldValue.Format(replacements);
    
    // newValue is "abcdeee".

### Concats(params string[] values)

Concatenates strings.

Example:

    var result = "123".Concats("456", "789", "abc");
    
    // result is "123456789abc".

### ToBase64()

Encode a string to Base64.（Encoding is UTF8）

Example:

    var result = "許功蓋".ToBase64();
    
    // result is "6Kix5Yqf6JOL"

### ToBase64(Encoding encoding)

Encode a string to Base64.

Example:

    var result = "許功蓋".ToBase64(Encoding.GetEncoding("Big5"));
    
    // result is "s1ylXLtc"

### Split(params string[] separator)

Split a string using string separators.

Example:

    var result = "1,,,,2,,3,,4".Split(",,");
    
    // result is ["1","","2","3","4"]

### SplitOmitEmptyEntries(params char[] separator)

Split a string without empty entries using char separators.

Example:

    var result = "1,,2,3,4".SplitOmitEmptyEntries(',');
    
    // result is ["1","2","3","4"]

### SplitOmitEmptyEntries(params string[] separator)

Split a string without empty entries using string separators.

Example:

    var result = "1,,,,2,,3,,4".SplitOmitEmptyEntries(",,");
    
    // result is ["1","2","3","4"]

### IsMatch(string pattern, , RegexOptions options = RegexOptions.IgnoreCase)

Return true if regular expression matched, otherwise return false. Default is case insensitive.

Example:

    var result = "qwerTYUiop".IsMatch("tyu");
    
    // result is true.

### Match(string pattern, RegexOptions options = RegexOptions.IgnoreCase)

Return regular expression matched result. Default is case insensitive.

Example:

    var match = "qwerTYUiop".Match("tyu");

### Matches(string pattern, RegexOptions options = RegexOptions.IgnoreCase)

Return regular expression matched results. Default is case insensitive.

Example:

    var matches = "qwtyuerTYUiop".Matches("tyu");
    
    // matches.Length is 2.

### ParseEnum&lt;T&gt;()

Case insensitive parse string to enum type.

Example:

    public enum MachineStatus
    {
        Provisioning,
        Staging,
        Running,
        Stopping,
        Terminated
    }
    
    var result = "STAGING".ParseEnum<MachineStatus>();
    
    // result is MachineStatus.Staging

### ToDateTime()

It return result of `DateTime.Parse(s)`.

Example:

    var result = "2019-02-18T15:00:42.984-08:00".ToDateTime();
    
    // result is 2019-02-18 07:00:42.984

### ToDateTime(string format)

It return result of `DateTime.ParseExact(s, format, CultureInfo.InvariantCulture)`.

Example:

    var result = "20180112123456".ToDateTime("yyyyMMddHHmmss");
    
    // result is 2018-01-12 12:34:56

### ToInt32()

Parse string to int.

Example:

    var result = "123".ToInt32();
    
    // result is 123.

### ToInt64()

Parse string to long.

Example:

    var result = "123456".ToInt64();
    
    // result is 123456.

### ToDouble()

Parse string to double.

Example:

    var result = "123.456".ToDouble();
    
    // result is 123.456.

## Chef.Extensions.Type

### IsUserDefined()

Check if Type is user defined.

Example:

    public class Person
    {
        public string Name { get; set; }
    
        public int Age { get; set; }
    }

    var result1 = typeof(Person).IsUserDefined();
    var result2 = typeof(string).IsUserDefined();
    
    // result1 is true, result2 is false.

### GetPropertyNames()

Return property names.

Example:

    var result = new { Name = "1", Age = 2 }.GetType().GetPropertyNames();
    
    // result is ["Name","Age"]

### GetPropertyNames(string prefix)

Return property names concatenated prefix.

Example:

    var result = new { Name = "1", Age = 2 }.GetType().GetPropertyNames("kkk.");
    
    // result is ["kkk.Name","kkk.Age"]

### GetActivator()

Return activator of type.

Example:

    var activator = typeof(List<int>).GetActivator();
    
    var list = (List<int>)activator();
    
    // list is a new instance of List<int>. 