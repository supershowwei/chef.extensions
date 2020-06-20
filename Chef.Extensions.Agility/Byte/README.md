## Chef.Extensions.Byte

### GetRange(int startIndex, int length)

Copy range of byte arrary.

Example:

    var data = new byte[] { 0x01, 0x02, 0x03 0x04, 0x05 };
    
    var bytes = data.GetRange(1, 2);
    
    // bytes is [0x02, 0x03].

### TryGetRange(int startIndex, int length, out byte[] bytes)

Try copy range of byte arrary.

Example:

    var data = new byte[] { 0x01, 0x02, 0x03 0x04, 0x05 };
    
    var result = data.TryGetRange(1, 2, out var bytes);
    
    // result is true.
    // bytes is [0x02, 0x03].

### GetRange(long startIndex, long length)

Copy range of byte arrary.

Example:

    var data = new byte[] { 0x01, 0x02, 0x03 0x04, 0x05 };
    
    var bytes = data.GetRange(2, 2);
    
    // bytes is [0x03, 0x04].

### TryGetRange(long startIndex, long length, out byte[] bytes)

Try copy range of byte arrary.

Example:

    var data = new byte[] { 0x01, 0x02, 0x03 0x04, 0x05 };
    
    var result = data.GetRange(2, 2, out var bytes);
    
    // result is true.
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


### ToUpperString()

Same as `BitConverter.ToString("bytes").Replace("-", string.Empty)`.

Example:

    var result = Encoding.UTF8.GetBytes("abcde~!@#$%").ToUpperString();
    
    // result is "61626364657E2140232425".

### ToLowerString()

Same as `BitConverter.ToString("bytes").Replace("-", string.Empty).ToLower()`.

Example:

    var result = Encoding.UTF8.GetBytes("abcde~!@#$%").ToLowerString();
    
    // result is "61626364657e2140232425".
