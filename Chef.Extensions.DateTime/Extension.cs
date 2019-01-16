using System;
using System.Globalization;

namespace Chef.Extensions.DateTime
{
    public static class Extension
    {
        public static System.DateTime SetYear(this System.DateTime me, int year)
        {
            return new System.DateTime(year, me.Month, me.Day, me.Hour, me.Minute, me.Second);
        }

        public static System.DateTime SetMonth(this System.DateTime me, int month)
        {
            return new System.DateTime(me.Year, month, me.Day, me.Hour, me.Minute, me.Second);
        }

        public static System.DateTime SetDay(this System.DateTime me, int day)
        {
            return new System.DateTime(me.Year, me.Month, day, me.Hour, me.Minute, me.Second);
        }

        public static System.DateTime SetHour(this System.DateTime me, int hour)
        {
            return new System.DateTime(me.Year, me.Month, me.Day, hour, me.Minute, me.Second);
        }

        public static System.DateTime SetMinute(this System.DateTime me, int minute)
        {
            return new System.DateTime(me.Year, me.Month, me.Day, me.Hour, minute, me.Second);
        }

        public static System.DateTime SetSecond(this System.DateTime me, int second)
        {
            return new System.DateTime(me.Year, me.Month, me.Day, me.Hour, me.Minute, second);
        }

        public static System.DateTime SpecifyDate(this System.DateTime me, int year, int month, int day)
        {
            return new System.DateTime(year, month, day, me.Hour, me.Minute, me.Second);
        }

        public static System.DateTime SpecifyDate(this System.DateTime me, System.DateTime date)
        {
            return new System.DateTime(date.Year, date.Month, date.Day, me.Hour, me.Minute, me.Second);
        }

        public static System.DateTime SpecifyTime(this System.DateTime me, int hour, int minute, int second)
        {
            return new System.DateTime(me.Year, me.Month, me.Day, hour, minute, second);
        }

        public static System.DateTime SpecifyTime(this System.DateTime me, System.DateTime time)
        {
            return new System.DateTime(me.Year, me.Month, me.Day, time.Hour, time.Minute, time.Second);
        }

        public static System.DateTime ToDateTime(this string me, string format)
        {
            return System.DateTime.ParseExact(me, format, CultureInfo.InvariantCulture);
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
    }
}