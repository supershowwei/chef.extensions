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