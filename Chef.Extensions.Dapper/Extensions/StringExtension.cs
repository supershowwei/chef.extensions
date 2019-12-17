namespace Chef.Extensions.Dapper.Extensions
{
    internal static class StringExtension
    {
        public static bool IsLikeOperator(this string me)
        {
            if (me.Equals("System.String.Contains")) return true;
            if (me.Equals("System.String.StartsWith")) return true;
            if (me.Equals("System.String.EndsWith")) return true;

            return false;
        }
    }
}