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

### Exists&lt;Ta, Tb&gt;(IEnumerable&lt;Tb&gt; collection, Func&lt;Ta, Tb, bool&gt; predicate)

Check if object is in collection

Example:

    var result = 1.Exists(new[] { 1, 2, 3 }, (a, b) => a == b);
    
    // result is true.

### NotExists&lt;Ta, Tb&gt;(IEnumerable&lt;Tb&gt; collection, Func&lt;Ta, Tb, bool&gt; predicate)

Check if object is not in collection

Example:

    var result = 1.NotExists(new[] { 1, 2, 3 }, (a, b) => a == b);
    
    // result is false.

### To&lt;TTarget&gt;()

Simple Object-Object Mapping.

Example:

    A a = new A();
    
    var result = a.To<B>();
    
    // result is B that properties same as A are already set.

### To&lt;TTarget&gt;(TTarget existed)

Simple Object-Object Mapping, but keep properties with non-default value in existed.

Example:

    A a = new A();
    
    var result = a.To<B>();
    
    // result is B that properties same as A are already set, but keep properties with non-default value in B.

### To&lt;TSource, TTarget&gt;(Func&lt;TSource, TTarget&gt; convert)

Simple Object-Object Mapping using custom converter.

Example:

    A a = new A();
    
    var result = a.To(x => new B());
    
    // result is B using "x => new B()" delegate function.

