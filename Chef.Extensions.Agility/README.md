# Chef.Extensions.Agility

A collection of useful extension methods without reference other packages.

## Chef.Extensions.Boolean

### public static T IIF<T>(this bool me, T trueValue, T falseValue)

Return `trueValue` if true, return `falseValue` if false.

### public static T IIF<T>(this bool me, Func<T> trueValue, Func<T> falseValue)

Execute and return `trueValue function` if true, execute and return `falseValue function` if false.

## Chef.Extensions.DateTime

### public static DateTime SetYear(this DateTime me, int year)

Change `Year` of DateTime.

### public static DateTime SetMonth(this DateTime me, int month)

Change `Month` of DateTime.

### public static DateTime SetDay(this DateTime me, int day)

Change `Day` of DateTime.

### public static DateTime SetHour(this DateTime me, int hour)

Change `Hour` of DateTime.

### public static DateTime SetMinute(this DateTime me, int minute)

Change `Minute` of DateTime.

### public static DateTime SetSecond(this DateTime me, int second)

Change `Second` of DateTime.

### public static DateTime StopYear(this DateTime me)

Keep `Year` value and set others as initail value.

Example:

    var datetime = new DateTime(2018, 12, 21, 12, 31, 44, 789).StopYear();
    
    // datetime is 2018/01/01 00:00:00.000

### public static DateTime StopMonth(this DateTime me)

Keep `Year` and `Month` value and set others as initail value.

Example:

    var datetime = new DateTime(2018, 12, 21, 12, 31, 44, 789).StopMonth();
    
    // datetime is 2018/12/01 00:00:00.000

### public static DateTime StopDay(this DateTime me)

Keep `Year`, `Month` and `Day` value and set others as initail value. It equals `DateTime.Date`.

Example:

    var datetime = new DateTime(2018, 12, 21, 12, 31, 44, 789).StopDay();
    
    // datetime is 2018/12/21 00:00:00.000

### public static DateTime StopHour(this DateTime me)

Keep `Year`, `Month`, `Day` and `Hour` value and set others as initail value.

Example:

    var datetime = new DateTime(2018, 12, 21, 12, 31, 44, 789).StopHour();
    
    // datetime is 2018/12/21 12:00:00.000

### public static DateTime StopMinute(this DateTime me)

Keep `Year`, `Month`, `Day`, `Hour` and `Minute` value and set others as initail value.

Example:

    var datetime = new DateTime(2018, 12, 21, 12, 31, 44, 789).StopMinute();
    
    // datetime is 2018/12/21 12:31:00.000

### public static DateTime StopSecond(this DateTime me)

Keep `Year`, `Month`, `Day`, `Hour`, `Minute` and `Second` value and set others as initail value.

Example:

    var datetime = new DateTime(2018, 12, 21, 12, 31, 44, 789).StopSecond();
    
    // datetime is 2018/12/21 12:31:44.000

### public static DateTime SpecifyDate(this DateTime me, int year, int month, int day)

Change `Year`, `Month` and `Day`.

Example:

    var datetime = new DateTime(2018, 12, 21, 12, 31, 44, 789).SpecifyDate(2019, 11, 12);
    
    // datetime is 2019/11/12 12:31:44.789

### public static DateTime SpecifyDate(this DateTime me, DateTime date)

Change `Year`, `Month` and `Day`.

Example:

    var datetime = new DateTime(2018, 12, 21, 12, 31, 44, 789).SpecifyDate(new DateTime(2019, 11, 12, 1, 1, 1, 111));
    
    // datetime is 2019/11/12 12:31:44.789

### public static DateTime SpecifyTime(this DateTime me, int hour, int minute, int second, int millisecond = 0)

Change `Hour`, `Minute`, `Second` and `Millisecond`（Default is 0）.

Example:

    var datetime = new DateTime(2018, 12, 21, 12, 31, 44, 789).SpecifyTime(1, 1, 1, 111));
    
    // datetime is 2018/12/21 01:01:01.111

### public static DateTime SpecifyTime(this DateTime me, DateTime time)

Change `Hour`, `Minute`, `Second` and `Millisecond`.

Example:

    var datetime = new DateTime(2018, 12, 21, 12, 31, 44, 789).SpecifyTime(new DateTime(2019, 11, 12, 1, 1, 1, 111));
    
    // datetime is 2018/12/21 01:01:01.111

### public static DateTime ToDateTime(this string me, string format)

It return `DateTime.ParseExact(me, format, CultureInfo.InvariantCulture)`.

### public static int DiffYears(this DateTime me, DateTime value)

Get date difference in `Years`.

### public static int DiffMonths(this DateTime me, DateTime value)

Get date difference in `Months`.

### public static int DiffDays(this DateTime me, DateTime value)

Get date difference in `Days`.

### public static int DiffHours(this DateTime me, DateTime value)

Get date difference in `Hours`.

### public static int DiffMinutes(this DateTime me, DateTime value)

Get date difference in `Minutes`.

### public static int DiffSeconds(this DateTime me, DateTime value)

Get date difference in `Seconds`.

