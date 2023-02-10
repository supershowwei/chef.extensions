using System.Text.RegularExpressions;

namespace Chef.Extensions.Regex
{
    public static class Extension
    {
        public static bool Match(this System.Text.RegularExpressions.Regex me, string input, out Match match)
        {
            match = me.Match(input);

            return match.Success;
        }
    }
}