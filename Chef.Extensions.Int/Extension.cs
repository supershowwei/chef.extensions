namespace Chef.Extensions.Int
{
    public static class Extension
    {
        public static bool Between(this int me, int begin, int end, bool inclusiveEnd = false)
        {
            return inclusiveEnd ? me >= begin && me <= end : me >= begin && me < end;
        }

        public static bool ExclusiveBetween(this int me, int begin, int end)
        {
            return me > begin && me < end;
        }
    }
}