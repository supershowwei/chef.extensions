using System;
using System.Collections.Generic;

namespace Chef.Extensions.DateTime
{
    public static class Extension
    {
        private static readonly System.DateTime Jan1st1970 = new System.DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static bool Between(this System.DateTime me, System.DateTime begin, System.DateTime end, bool exclusiveEnd = false)
        {
            return exclusiveEnd ? me >= begin && me < end : me >= begin && me <= end;
        }

        public static bool ExclusiveBetween(this System.DateTime me, System.DateTime begin, System.DateTime end)
        {
            return me > begin && me < end;
        }

        public static System.DateTime SetYear(this System.DateTime me, int year)
        {
            return new System.DateTime(year, me.Month, me.Day, me.Hour, me.Minute, me.Second, me.Millisecond);
        }

        public static System.DateTime SetMonth(this System.DateTime me, int month)
        {
            return new System.DateTime(me.Year, month, me.Day, me.Hour, me.Minute, me.Second, me.Millisecond);
        }

        public static System.DateTime SetDay(this System.DateTime me, int day)
        {
            return new System.DateTime(me.Year, me.Month, day, me.Hour, me.Minute, me.Second, me.Millisecond);
        }

        public static System.DateTime SetHour(this System.DateTime me, int hour)
        {
            return new System.DateTime(me.Year, me.Month, me.Day, hour, me.Minute, me.Second, me.Millisecond);
        }

        public static System.DateTime SetMinute(this System.DateTime me, int minute)
        {
            return new System.DateTime(me.Year, me.Month, me.Day, me.Hour, minute, me.Second, me.Millisecond);
        }

        public static System.DateTime SetSecond(this System.DateTime me, int second)
        {
            return new System.DateTime(me.Year, me.Month, me.Day, me.Hour, me.Minute, second, me.Millisecond);
        }

        public static System.DateTime StopYear(this System.DateTime me)
        {
            return new System.DateTime(me.Year, 1, 1, 0, 0, 0, 0);
        }

        public static System.DateTime StopMonth(this System.DateTime me)
        {
            return new System.DateTime(me.Year, me.Month, 1, 0, 0, 0, 0);
        }

        public static System.DateTime StopDay(this System.DateTime me)
        {
            return new System.DateTime(me.Year, me.Month, me.Day, 0, 0, 0, 0);
        }

        public static System.DateTime StopHour(this System.DateTime me)
        {
            return new System.DateTime(me.Year, me.Month, me.Day, me.Hour, 0, 0, 0);
        }

        public static System.DateTime StopMinute(this System.DateTime me)
        {
            return new System.DateTime(me.Year, me.Month, me.Day, me.Hour, me.Minute, 0, 0);
        }

        public static System.DateTime StopSecond(this System.DateTime me)
        {
            return new System.DateTime(me.Year, me.Month, me.Day, me.Hour, me.Minute, me.Second, 0);
        }

        public static System.DateTime SetStopYear(this System.DateTime me, int year)
        {
            return new System.DateTime(year, 1, 1, 0, 0, 0, 0);
        }

        public static System.DateTime SetStopMonth(this System.DateTime me, int month)
        {
            return new System.DateTime(me.Year, month, 1, 0, 0, 0, 0);
        }

        public static System.DateTime SetStopDay(this System.DateTime me, int day)
        {
            return new System.DateTime(me.Year, me.Month, day, 0, 0, 0, 0);
        }

        public static System.DateTime SetStopHour(this System.DateTime me, int hour)
        {
            return new System.DateTime(me.Year, me.Month, me.Day, hour, 0, 0, 0);
        }

        public static System.DateTime SetStopMinute(this System.DateTime me, int minute)
        {
            return new System.DateTime(me.Year, me.Month, me.Day, me.Hour, minute, 0, 0);
        }

        public static System.DateTime SetStopSecond(this System.DateTime me, int second)
        {
            return new System.DateTime(me.Year, me.Month, me.Day, me.Hour, me.Minute, second, 0);
        }

        public static System.DateTime SpecifyDate(this System.DateTime me, int year, int month, int day)
        {
            return new System.DateTime(year, month, day, me.Hour, me.Minute, me.Second, me.Millisecond);
        }

        public static System.DateTime SpecifyDate(this System.DateTime me, System.DateTime date)
        {
            return new System.DateTime(date.Year, date.Month, date.Day, me.Hour, me.Minute, me.Second, me.Millisecond);
        }

        public static System.DateTime SpecifyTime(this System.DateTime me, int hour, int minute, int second, int millisecond = 0)
        {
            return new System.DateTime(me.Year, me.Month, me.Day, hour, minute, second, millisecond);
        }

        public static System.DateTime SpecifyTime(this System.DateTime me, System.DateTime time)
        {
            return new System.DateTime(me.Year, me.Month, me.Day, time.Hour, time.Minute, time.Second, time.Millisecond);
        }            

        public static int DiffYears(this System.DateTime me, System.DateTime value)
        {
            var diff = me.Year - value.Year;

            return Math.Abs(diff);
        }

        public static int DiffMonths(this System.DateTime me, System.DateTime value)
        {
            var diff = ((me.Year - value.Year) * 12) + me.Month - value.Month;

            return Math.Abs(diff);
        }

        public static int DiffDays(this System.DateTime me, System.DateTime value)
        {
            return Math.Abs(Convert.ToInt32(Math.Floor(me.Subtract(value).TotalDays)));
        }

        public static int DiffHours(this System.DateTime me, System.DateTime value)
        {
            return Math.Abs(Convert.ToInt32(Math.Floor(me.Subtract(value).TotalHours)));
        }

        public static int DiffMinutes(this System.DateTime me, System.DateTime value)
        {
            return Math.Abs(Convert.ToInt32(Math.Floor(me.Subtract(value).TotalMinutes)));
        }

        public static int DiffSeconds(this System.DateTime me, System.DateTime value)
        {
            return Math.Abs(Convert.ToInt32(Math.Floor(me.Subtract(value).TotalSeconds)));
        }

        public static long ToUnixTimestamp(this System.DateTime me)
        {
            return Convert.ToInt64(me.ToUniversalTime().Subtract(Jan1st1970).TotalMilliseconds);
        }

        public static System.DateTime DateOfThisWeek(this System.DateTime me, DayOfWeek dayOfWeek)
        {
            return me.AddDays(dayOfWeek - me.DayOfWeek).Date;
        }

        public static System.DateTime DateOfThisWeek(this System.DateTime me, DayOfWeek dayOfWeek, DayOfWeek startOfWeek)
        {
            if (startOfWeek == DayOfWeek.Sunday) return DateOfThisWeek(me, dayOfWeek);

            var newMeDayOfWeek = me.DayOfWeek - startOfWeek;
            if (newMeDayOfWeek < 0) newMeDayOfWeek += 7;

            var newDayOfWeek = dayOfWeek - startOfWeek;
            if (newDayOfWeek < 0) newDayOfWeek += 7;

            return me.AddDays(newDayOfWeek - newMeDayOfWeek).Date;
        }
    }
}