## Chef.Extensions.Decimal

### Round(MidpointRounding mode = MidpointRounding.AwayFromZero)

Round away from zero.

Example:

    var num1 = 1.4m.Round();
    var num2 = 1.5m.Round();
    var num3 = 1.6m.Round();
    
    // num1 = 1, num2 = 2, num3 = 2

### Round(int digits, MidpointRounding mode = MidpointRounding.AwayFromZero)

Round to `digits` places away from zero.

Example:

    var num1 = 1.44m.Round(1);
    var num2 = 1.55m.Round(1);
    var num3 = 1.66m.Round(1);
    
    // num1 = 1.4, num2 = 1.6, num3 = 1.7

### RoundUp()

Round up.

Example:

    var result = 1.44m.RoundUp();

    // result is 2.

### RoundUp(int digits)

Round up to `digits` places.

Example:

    var num1 = 1.44m.RoundUp(1);
    var num2 = 1.55m.RoundUp(1);
    var num3 = 1.66m.RoundUp(1);
    
    // num1 = 1.5, num2 = 1.6, num3 = 1.7

### RoundDown()

Round down.

Example:

    var result = 1.44m.RoundDown();

    // result is 1.

### RoundDown(int digits)

Round down to `digits` places.

Example:

    var num1 = 1.44m.RoundDown(1);
    var num2 = 1.55m.RoundDown(1);
    var num3 = 1.66m.RoundDown(1);
    
    // num1 = 1.4, num2 = 1.5, num3 = 1.6

### Truncate()

Calculates the integral part of a number.

Example:

    var num1 = 1.44m.Truncate();
    
    // num1 = 1

### ToInt32()

Convert a decimal to int.

Example:

    var num1 = 1.4m.ToInt32();
    var num2 = 1.5m.ToInt32();
    var num3 = 1.6m.ToInt32();
    
    // num1 = 1, num2 = 2, num3 = 2

### ToInt64()

Convert a decimal to long.

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
