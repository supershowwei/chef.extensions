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