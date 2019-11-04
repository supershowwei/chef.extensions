## Chef.Extensions.Decimal

### Round(MidpointRounding mode = MidpointRounding.AwayFromZero)

Round a int away from zero.

Example:

    var num1 = 1.4m.Round();
    var num2 = 1.5m.Round();
    var num3 = 1.6m.Round();
    
    // num1 = 1, num2 = 2, num3 = 2

### Round(int digits, MidpointRounding mode = MidpointRounding.AwayFromZero)

Round a double to `digits` places away from zero.

Example:

    var num1 = 1.44m.Round(1);
    var num2 = 1.55m.Round(1);
    var num3 = 1.66m.Round(1);
    
    // num1 = 1.4, num2 = 1.6, num3 = 1.7

### RoundUp()

Round up a double to int.

Example:

    var result = 1.44m.RoundUp();

    // result is 2.

### RoundUp(int digits)

Round up a double to `digits` places.

Example:

    var num1 = 1.44m.RoundUp(1);
    var num2 = 1.55m.RoundUp(1);
    var num3 = 1.66m.RoundUp(1);
    
    // num1 = 1.5, num2 = 1.6, num3 = 1.7

### RoundDown()

Round down a double to int.

Example:

    var result = 1.44m.RoundDown();

    // result is 1.

### RoundDown(int digits)

Round down a double to `digits` places.

Example:

    var num1 = 1.44m.RoundDown(1);
    var num2 = 1.55m.RoundDown(1);
    var num3 = 1.66m.RoundDown(1);
    
    // num1 = 1.4, num2 = 1.5, num3 = 1.6

### ToInt32()

Convert a double to int.

Example:

    var num1 = 1.4m.ToInt32();
    var num2 = 1.5m.ToInt32();
    var num3 = 1.6m.ToInt32();
    
    // num1 = 1, num2 = 2, num3 = 2

### ToInt64()

Convert a double to long.

Example:

    var num1 = 1.4m.ToInt64();
    var num2 = 1.5m.ToInt64();
    var num3 = 1.6m.ToInt64();
    
    // num1 = 1, num2 = 2, num3 = 2

### Gradient(decimal baseValue)

Calculate change rate with baseValue.

Example:

    var num = 11m;
    var baseValue = 10m;
    var result = num.Gradient(baseValue);
    
    // result is 0.1

### Normalize()

Remove trailing zeros.

Example:

    var num = 1.1000m;
    var result = num.Normalize();
    
    // result is 1.1
