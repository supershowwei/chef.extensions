using System;

namespace Chef.Extensions.Decimal
{
    public static class Extension
    {
        public static decimal Round(this decimal me, MidpointRounding mode = MidpointRounding.AwayFromZero)
        {
            return Math.Round(me, mode);
        }

        public static decimal Round(this decimal me, int digits, MidpointRounding mode = MidpointRounding.AwayFromZero)
        {
            return Math.Round(me, digits, mode);
        }

        public static decimal RoundUp(this decimal me)
        {
            return Math.Ceiling(me);
        }

        public static decimal RoundUp(this decimal me, int digits)
        {
            var power = (decimal)Math.Pow(10, digits);

            return Math.Ceiling(me * power) / power;
        }

        public static decimal RoundDown(this decimal me)
        {
            return Math.Floor(me);
        }

        public static decimal RoundDown(this decimal me, int digits)
        {
            var power = (decimal)Math.Pow(10, digits);

            return Math.Floor(me * power) / power;
        }

        public static decimal Truncate(this decimal me)
        {
            return Math.Truncate(me);
        }

        public static int ToInt32(this decimal me)
        {
            return Convert.ToInt32(me);
        }

        public static long ToInt64(this decimal me)
        {
            return Convert.ToInt64(me);
        }

        public static decimal Gradient(this decimal me, decimal baseValue)
        {
            return (me - baseValue) / baseValue;
        }

        public static decimal Normalize(this decimal me)
        {
            return me / 1.0000000000000000000000000000m;
        }
    }
}