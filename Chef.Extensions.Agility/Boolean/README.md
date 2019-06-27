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