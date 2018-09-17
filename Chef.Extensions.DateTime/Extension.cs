using System.Globalization;

namespace Chef.Extensions.DateTime
{
    public static class Extension
    {
        public static System.DateTime SpecifyTime(this System.DateTime me, int hour, int minute, int second)
        {
            return new System.DateTime(me.Year, me.Month, me.Day, hour, minute, second);
        }

        public static System.DateTime ToDateTime(this string me, string format)
        {
            return System.DateTime.ParseExact(me, format, CultureInfo.InvariantCulture);
        }
    }
}