using System;

namespace Chef.Extensions.Float
{
    public static class Extension
    {
        public static int Round(this float me, MidpointRounding mode = MidpointRounding.AwayFromZero)
        {
            return Convert.ToInt32(Math.Round(me, mode));
        }

        public static float Round(this float me, int digits, MidpointRounding mode = MidpointRounding.AwayFromZero)
        {
            var power = Convert.ToInt32(Math.Pow(10, digits));

            return Convert.ToSingle(Math.Round(me * power, mode)) / power;
        }

        public static int RoundUp(this float me)
        {
            return Convert.ToInt32(Math.Ceiling(me));
        }

        public static float RoundUp(this float me, int digits)
        {
            var power = Convert.ToInt32(Math.Pow(10, digits));

            return Convert.ToSingle(Math.Ceiling(me * power)) / power;
        }

        public static int RoundDown(this float me)
        {
            return Convert.ToInt32(Math.Floor(me));
        }

        public static float RoundDown(this float me, int digits)
        {
            var power = Convert.ToInt32(Math.Pow(10, digits));

            return Convert.ToSingle(Math.Floor(me * power)) / power;
        }

        public static int ToInt32(this float me)
        {
            return Convert.ToInt32(me);
        }

        public static long ToInt64(this float me)
        {
            return Convert.ToInt64(me);
        }
    }
}