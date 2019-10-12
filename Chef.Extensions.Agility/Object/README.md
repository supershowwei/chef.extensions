## Chef.Extensions.Object

### IsNotNull()

Return true if object is null, otherwise false.

Example:

    var result = new object().IsNotNull();
    
    // result is true.

### To&lt;T&gt;()

Change object type to T.

Example:

    var result = 1.To<string>();
    
    // result is "1".

### ToNullable&lt;T&gt;() where T : struct

Change object of struct type to Nullable&lt;T&gt;.

Example:

    var result1 = 1.ToNullable<long>();
    var result2 = ((object)null).ToNullable<int>();
    
    // result1 is 1 and type is long?.
    // result2 is null and type is int?.

### ToExpando()

Convert a object to `ExpandoObject`.

Example:

    var result = new { Name = "abc", Age = 2 }.ToExpando();
    
    // result is {"Name":"abc","Age":2}

### IsNumeric()

Check if object is numeric.

Exmaple:

    var result = 1.IsNumeric();
    
    // result = is true.
