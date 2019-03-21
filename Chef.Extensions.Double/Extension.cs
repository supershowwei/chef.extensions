using System;

namespace Chef.Extensions.Double
{
    public static class Extension
    {
        public static int Round(this double me, MidpointRounding mode = MidpointRounding.AwayFromZero)
        {
            return Convert.ToInt32(Math.Round(me, mode));
        }

        public static double Round(this double me, int digits, MidpointRounding mode = MidpointRounding.AwayFromZero)
        {
            return Math.Round(me, digits, mode);
        }

        public static int RoundUp(this double me)
        {
            return Convert.ToInt32(Math.Ceiling(me));
        }

        public static double RoundUp(this double me, int digits)
        {
            var power = Math.Pow(10, digits);

            return Math.Ceiling(me * power) / power;
        }

        public static int RoundDown(this double me)
        {
            return Convert.ToInt32(Math.Floor(me));
        }

        public static double RoundDown(this double me, int digits)
        {
            var power = Math.Pow(10, digits);

            return Math.Floor(me * power) / power;
        }

        public static int ToInt(this double me)
        {
            return Convert.ToInt32(me);
        }

        public static long ToLong(this double me)
        {
            return Convert.ToInt64(me);
        }
    }
}