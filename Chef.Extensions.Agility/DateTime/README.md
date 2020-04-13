## Chef.Extensions.DateTime

### Between(DateTime begin, DateTime end, bool exclusiveEnd = false)

Check if datetime is between begin and end.

Example:

    var result1 = new DateTime(2018, 6, 1).Between(new DateTime(2018, 1, 1), new DateTime(2018, 6, 1));
    var result2 = new DateTime(2018, 6, 1).Between(new DateTime(2018, 1, 1), new DateTime(2018, 6, 1), true);
    
    // result1 is true.
    // result2 is false.

### ExclusiveBetween(DateTime begin, DateTime end)

Check if datetime is between begin and end exclusively.

Example:

    var result = new DateTime(2018, 6, 1).ExclusiveBetween(new DateTime(2018, 6, 1), new DateTime(2018, 6, 1));

    // result is false.

### SetYear(int year)

Change `Year` of DateTime.

Example:

    var result = new DateTime(2018, 1, 12, 12, 34, 56).SetYear(2017);
    
    // result is 2017/1/12 12:34:56

### SetMonth(int month)

Change `Month` of DateTime.

Example:

    var result = new DateTime(2018, 1, 12, 12, 34, 56).SetMonth(2);
    
    // result is 2018/2/12 12:34:56

### SetDay(int day)

Change `Day` of DateTime.

Example:

    var result = new DateTime(2018, 1, 12, 12, 34, 56).SetDay(13);
    
    // result is 2018/1/13 12:34:56

### SetHour(int hour)

Change `Hour` of DateTime.

Example:

    var result = new DateTime(2018, 1, 12, 12, 34, 56).SetHour(14);
    
    // result is 2018/1/12 14:34:56

### SetMinute(int minute)

Change `Minute` of DateTime.

Example:

    var result = new DateTime(2018, 1, 12, 12, 34, 56).SetMinute(45);
    
    // result is 2018/1/12 12:45:56

### SetSecond(int second)

Change `Second` of DateTime.

Example:

    var result = new DateTime(2018, 1, 12, 12, 34, 56).SetSecond(23);
    
    // result is 2018/1/12 12:34:23

### StopYear()

Keep `Year` value and set others as initail value.

Example:

    var datetime = new DateTime(2018, 12, 21, 12, 31, 44, 789).StopYear();
    
    // datetime is 2018/01/01 00:00:00.000

### StopMonth()

Keep `Year`, `Month` value and set others as initail value.

Example:

    var datetime = new DateTime(2018, 12, 21, 12, 31, 44, 789).StopMonth();
    
    // datetime is 2018/12/01 00:00:00.000

### StopDay()

Keep `Year`, `Month`, `Day` value and set others as initail value. It equals `DateTime.Date`.

Example:

    var datetime = new DateTime(2018, 12, 21, 12, 31, 44, 789).StopDay();
    
    // datetime is 2018/12/21 00:00:00.000

### StopHour()

Keep `Year`, `Month`, `Day`, `Hour` value and set others as initail value.

Example:

    var datetime = new DateTime(2018, 12, 21, 12, 31, 44, 789).StopHour();
    
    // datetime is 2018/12/21 12:00:00.000

### StopMinute()

Keep `Year`, `Month`, `Day`, `Hour` and `Minute` value and set others as initail value.

Example:

    var datetime = new DateTime(2018, 12, 21, 12, 31, 44, 789).StopMinute();
    
    // datetime is 2018/12/21 12:31:00.000

### StopSecond()

Keep `Year`, `Month`, `Day`, `Hour`, `Minute`, `Second` value and set others as initail value.

Example:

    var datetime = new DateTime(2018, 12, 21, 12, 31, 44, 789).StopSecond();
    
    // datetime is 2018/12/21 12:31:44.000

### SetStopYear(int year)

Change `Year` of DateTime, keep `Year` value and set others as initail value.

Example:

    var result = new DateTime(2018, 1, 12, 12, 34, 56).SetStopYear(2017);
    
    // result is 2017/1/1 00:00:00

### SetStopMonth(int month)

Change `Month` of DateTime, keep `Year`, `Month` value and set others as initail value.

Example:

    var result = new DateTime(2018, 1, 12, 12, 34, 56).SetStopMonth(2);
    
    // result is 2018/2/1 00:00:00

### SetStopDay(int day)

Change `Day` of DateTime, keep `Year`, `Month`, `Day` value and set others as initail value. It equals `DateTime.Date`.

Example:

    var result = new DateTime(2018, 1, 12, 12, 34, 56).SetStopDay(13);
    
    // result is 2018/1/13 00:00:00

### SetStopHour(int hour)

Change `Hour` of DateTime, keep `Year`, `Month`, `Day`, `Hour` value and set others as initail value.

Example:

    var result = new DateTime(2018, 1, 12, 12, 34, 56).SetStopHour(14);
    
    // result is 2018/1/12 14:00:00

### SetStopMinute(int minute)

Change `Minute` of DateTime, keep `Year`, `Month`, `Day`, `Hour` and `Minute` value and set others as initail value.

Example:

    var result = new DateTime(2018, 1, 12, 12, 34, 56).SetStopMinute(45);
    
    // result is 2018/1/12 12:45:00

### SetStopSecond(int second)

Change `Second` of DateTime, keep `Year`, `Month`, `Day`, `Hour`, `Minute`, `Second` value and set others as initail value.

Example:

    var result = new DateTime(2018, 1, 12, 12, 34, 56).SetStopSecond(23);
    
    // result is 2018/1/12 12:34:23.000

### SpecifyDate(int year, int month, int day)

Change `Year`, `Month` and `Day`.

Example:

    var datetime = new DateTime(2018, 12, 21, 12, 31, 44, 789).SpecifyDate(2019, 11, 12);
    
    // datetime is 2019/11/12 12:31:44.789

### SpecifyDate(DateTime date)

Change `Year`, `Month`, `Day`.

Example:

    var datetime = new DateTime(2018, 12, 21, 12, 31, 44, 789).SpecifyDate(new DateTime(2019, 11, 12, 1, 1, 1, 111));
    
    // datetime is 2019/11/12 12:31:44.789

### SpecifyTime(int hour, int minute, int second, int millisecond = 0)

Change `Hour`, `Minute`, `Second`, `Millisecond`（Default is 0）.

Example:

    var datetime = new DateTime(2018, 12, 21, 12, 31, 44, 789).SpecifyTime(1, 1, 1, 111));
    
    // datetime is 2018/12/21 01:01:01.111

### SpecifyTime(DateTime time)

Change `Hour`, `Minute`, `Second`, `Millisecond`.

Example:

    var datetime = new DateTime(2018, 12, 21, 12, 31, 44, 789).SpecifyTime(new DateTime(2019, 11, 12, 1, 1, 1, 111));
    
    // datetime is 2018/12/21 01:01:01.111

### DiffYears(DateTime value)

Get date difference in `Years`.

Example:

    var result = new DateTime(2018, 1, 2).DiffYears(new DateTime(2019, 2, 1));
    
    // result is 1.

### DiffMonths(DateTime value)

Get date difference in `Months`.

Example:

    var result = new DateTime(2018, 1, 2).DiffMonths(new DateTime(2019, 2, 1));
    
    // result is 13.

### DiffDays(DateTime value)

Get date difference in `Days`.

Example:

    var result = new DateTime(2018, 1, 2).DiffDays(new DateTime(2019, 2, 1));
    
    // result is 395.

### DiffHours(DateTime value)

Get date difference in `Hours`.

Example:

    var result = new DateTime(2018, 1, 2, 12, 34, 56).DiffHours(new DateTime(2019, 2, 1, 1, 23, 45));
    
    // result is 9469

### DiffMinutes(DateTime value)

Get date difference in `Minutes`.

Example:

    var result = new DateTime(2018, 1, 2, 12, 34, 56).DiffMinutes(new DateTime(2019, 2, 1, 1, 23, 45));
    
    // result is 568129.

### DiffSeconds(DateTime value)

Get date difference in `Seconds`.

Example:

    var result = new DateTime(2018, 1, 2, 12, 34, 56).DiffSeconds(new DateTime(2019, 2, 1, 1, 23, 45));
    
    // result is 34087729.

### ToUnixTimestamp()

Convert to milliseconds of Unix time.

Example:

    var time = new DateTime(2019, 5, 7, 10, 23, 55);
    
    var unixTimeMs = time.ToUnixTimeMilliseconds();
    
    // unixTimeMs is 1557224635000.

### DateOfThisWeek(DayOfWeek dayOfWeek)

Get date of DayOfWeek in this week. (Start of week is Sunday)

Example:

    var date = new DateTime(2019, 7, 5);
    
    var dateOfWeek = date.DateOfThisWeek(DayOfWeek.Thursday);
    
    // dateOfWeek is "2019-07-04".

### DateOfThisWeek(DayOfWeek dayOfWeek, DayOfWeek startOfWeek)

Get date of DayOfWeek in this week. (Start of week is startOfWeek)

Example:

    var date = new DateTime(2019, 7, 5);
    
    var dateOfWeek = date.DateOfThisWeek(DayOfWeek.Thursday, DayOfWeek.Friday);
    
    // dateOfWeek is "2019-07-11".
