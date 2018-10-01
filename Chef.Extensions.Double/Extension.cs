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

        public static int ToInt32(this double me)
        {
            return Convert.ToInt32(me);
        }

        public static long ToInt64(this double me)
        {
            return Convert.ToInt64(me);
        }
    }
}