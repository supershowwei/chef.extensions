using System;

namespace Chef.Extensions.Double
{
    public static class Extension
    {
        public static double Round(this double me, MidpointRounding mode = MidpointRounding.AwayFromZero)
        {
            return Math.Round(me, mode);
        }

        public static double Round(this double me, int digits, MidpointRounding mode = MidpointRounding.AwayFromZero)
        {
            return Math.Round(me, digits, mode);
        }

        public static double RoundUp(this double me)
        {
            return Math.Ceiling(me);
        }

        public static double RoundUp(this double me, int digits)
        {
            var power = Math.Pow(10, digits);

            return Math.Ceiling(me * power) / power;
        }

        public static double RoundDown(this double me)
        {
            return Math.Floor(me);
        }

        public static double RoundDown(this double me, int digits)
        {
            var power = Math.Pow(10, digits);

            return Math.Floor(me * power) / power;
        }

        public static double Truncate(this double me)
        {
            return Math.Truncate(me);
        }

        public static int ToInt32(this double me)
        {
            return Convert.ToInt32(me);
        }

        public static long ToInt64(this double me)
        {
            return Convert.ToInt64(me);
        }

        public static double Gradient(this double me, double baseValue)
        {
            return (me - baseValue) / baseValue;
        }
    }
}