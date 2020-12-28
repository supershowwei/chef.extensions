## Chef.Extensions.Float

### Round(MidpointRounding mode = MidpointRounding.AwayFromZero)

Round a int away from zero.

Example:

    var num1 = 1.4f.Round();
    var num2 = 1.5f.Round();
    var num3 = 1.6f.Round();
    
    // num1 = 1, num2 = 2, num3 = 2

### Round(int digits, MidpointRounding mode = MidpointRounding.AwayFromZero)

Round a float to `digits` places away from zero.

Example:

    var num1 = 1.44f.Round(1);
    var num2 = 1.55f.Round(1);
    var num3 = 1.66f.Round(1);
    
    // num1 = 1.4, num2 = 1.6, num3 = 1.7

### RoundUp()

Round up a float to int.

Example:

    var result = 1.44f.RoundUp();

    // result is 2.

### RoundUp(int digits)

Round up a float to `digits` places.

Example:

    var num1 = 1.44f.RoundUp(1);
    var num2 = 1.55f.RoundUp(1);
    var num3 = 1.66f.RoundUp(1);
    
    // num1 = 1.5, num2 = 1.6, num3 = 1.7

### RoundDown()

Round down a float to int.

Example:

    var result = 1.44f.RoundDown();

    // result is 1.

### RoundDown(int digits)

Round down a float to `digits` places.

Example:

    var num1 = 1.44f.RoundDown(1);
    var num2 = 1.55f.RoundDown(1);
    var num3 = 1.66f.RoundDown(1);
    
    // num1 = 1.4, num2 = 1.5, num3 = 1.6

### ToInt32()

Convert a float to int.

Example:

    var num1 = 1.4f.ToInt32();
    var num2 = 1.5f.ToInt32();
    var num3 = 1.6f.ToInt32();
    
    // num1 = 1, num2 = 2, num3 = 2

### ToInt64()

Convert a float to long.

Example:

    var num1 = 1.4f.ToInt64();
    var num2 = 1.5f.ToInt64();
    var num3 = 1.6f.ToInt64();
    
    // num1 = 1, num2 = 2, num3 = 2

