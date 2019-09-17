## Chef.Extensions.Int

### Between(int begin, int end, bool exclusiveEnd = false)

Check if int value is between begin and end.

Example:

    var result1 = 10.Between(1, 10);
    var result2 = 10.Between(1, 10, true);
    
    // result1 is true.
    // result2 is false.

### ExclusiveBetween(int begin, int end)

Check if datetime is between begin and end exclusively.

Example:

    var result = 1.ExclusiveBetween(1, 1);

    // result is false.
