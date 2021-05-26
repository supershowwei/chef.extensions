using System;

namespace Chef.Extensions.Float
{
    public static class Extension
    {
        public static float Round(this float me, MidpointRounding mode = MidpointRounding.AwayFromZero)
        {
            return Convert.ToSingle(Math.Round(me, mode));
        }

        public static float Round(this float me, int digits, MidpointRounding mode = MidpointRounding.AwayFromZero)
        {
            return Convert.ToSingle(Math.Round(me, digits, mode));
        }

        public static float RoundUp(this float me)
        {
            return Convert.ToSingle(Math.Ceiling(me));
        }

        public static float RoundUp(this float me, int digits)
        {
            var power = Math.Pow(10, digits);

            return Convert.ToSingle(Math.Ceiling(me * power) / power);
        }

        public static float RoundDown(this float me)
        {
            return Convert.ToSingle(Math.Floor(me));
        }

        public static float RoundDown(this float me, int digits)
        {
            var power = Math.Pow(10, digits);

            return Convert.ToSingle(Math.Floor(me * power) / power);
        }

        public static float Truncate(this float me)
        {
            return Convert.ToSingle(Math.Truncate(me));
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