using System.Text.RegularExpressions;

namespace Chef.Extensions.Controller
{
    internal class CacheViewIdentity
    {
        public string CacheKey { get; set; }

        public string Checksum { get; set; }

        public static CacheViewIdentity Create(string ifNoneMatch)
        {
            if (string.IsNullOrEmpty(ifNoneMatch)) return null;

            var match = Regex.Match(ifNoneMatch, "([0-9a-f]+)-([0-9a-f]+)");

            if (!match.Success) return null;

            return new CacheViewIdentity { CacheKey = match.Groups[1].Value, Checksum = match.Groups[2].Value };
        }
    }
}