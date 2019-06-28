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

### ToBase32()

Encode a string to Base32.（Encoding is UTF8）

Example:

    var result = "許功蓋".ToBase32();
    
    // result is "5CULDZMKT7UJHCY"

### ToBase32(Encoding encoding)

Encode a string to Base32.

Example:

    var result = "許功蓋".ToBase32(Encoding.GetEncoding("Big5"));
    
    // result is "WNOKKXF3LQ"

### Base32Decode()

Decode a string of Base32 to string.（Encoding is UTF8）

Example:

    var result = "MFRGG".Base32Decode();
    
    // result is "abc".

### Base32Decode(Encoding encoding)

Decode a string of Base32 to string.

Example:

    var result = "MFRGG".Base32Decode(Encoding.GetEncoding("Big5"));
    
    // result is "abc".

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

### Base64Decode()

Decode a string of Base64 to string.（Encoding is UTF8）

Example:

    var result = "YWJj".Base64Decode();
    
    // result is "abc".

### Base64Decode(Encoding encoding)

Decode a string of Base64 to string.

Example:

    var result = "MFRGG".Base64Decode(Encoding.GetEncoding("Big5"));
    
    // result is "abc".

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