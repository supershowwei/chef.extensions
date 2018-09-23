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
    }
}